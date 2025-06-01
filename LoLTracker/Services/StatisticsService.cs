using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using LoLTracker.Models.Dto;

namespace LoLTracker.Services
{
    internal class StatisticsService(CacheService cache)
    {
        public async Task<MatchReport> CalculateStats(string riotId, int iterations = 2)
        {
            var riotAccount = await cache.GetRiotAccountDetailsAsync(riotId);
            var currentMatchHistory = await cache.GetMatchIdsAsync(riotAccount.Puuid, 0);
            var lastMatch = await cache.GetMatchInfoAsync(currentMatchHistory[0]);

            var allyTeam = new TeamStats();
            var enemyTeam = new TeamStats();

            Dictionary<string, PlayerStats> statsByPlayers = [];
            HashSet<string> processedMatches = [currentMatchHistory[0]];

            Dictionary<string, string[]> matchHistoryIndices = [];
            foreach (var participant in lastMatch.Participants)
            {
                var participantTeam = participant.TeamId switch
                {
                    TeamDto.MatchTeam.Blue => allyTeam,
                    TeamDto.MatchTeam.Red => enemyTeam,
                    _ => ThrowHelper.ThrowInvalidOperationException<TeamStats>()
                };

                var playerStats = new PlayerStats
                {
                    PlayerName = $"{participant.RiotIdGameName}#{participant.RiotIdTagLine}",
                    ChampionName = participant.ChampionName,
                    ChampionId = participant.ChampionId,
                };
                participantTeam.Players.Add(playerStats);
                statsByPlayers[participant.Puuid] = playerStats;

                matchHistoryIndices[participant.Puuid] = await cache.GetMatchIdsAsync(participant.Puuid, 0);
            }

            int maxMatchesToProcess = Math.Min(iterations, matchHistoryIndices.Values.Max(indices => indices.Length));

            foreach (var participant in lastMatch.Participants)
            {
                var indices = matchHistoryIndices[participant.Puuid];
                var participantStats = statsByPlayers[participant.Puuid];
                for (int i = 0; participantStats.MatchCount < maxMatchesToProcess; i++)
                {
                    if (indices.Length <= i) continue;
                    var trackedMatchId = indices[i];
                    if (processedMatches.Contains(trackedMatchId)) continue;

                    var trackedMatch = await cache.GetMatchInfoAsync(trackedMatchId);
                    processedMatches.Add(trackedMatchId);

                    foreach (var champion in trackedMatch.Participants)
                    {
                        if (statsByPlayers.TryGetValue(champion.Puuid, out var stats))
                        {
                            var efficiency = EfficiencyCalculator.CalculateEfficiency(champion, trackedMatch);
                            stats.Efficiency += efficiency;
                            stats.MatchCount++;
                        }
                    }
                }
            }

            foreach (var playerStats in statsByPlayers.Values)
            {
                if (playerStats.MatchCount > 0)
                {
                    playerStats.Efficiency /= playerStats.MatchCount;
                }
            }

            allyTeam.TotalEfficiency = allyTeam.Players.Sum(x => x.Efficiency);
            enemyTeam.TotalEfficiency = enemyTeam.Players.Sum(x => x.Efficiency);

            return new(allyTeam, enemyTeam)
            {
                WinProbability = EfficiencyCalculator.CalculateWinProbability(allyTeam, enemyTeam)
            };
        }
    }
}