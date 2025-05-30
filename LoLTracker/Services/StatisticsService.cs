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
        public async Task<MatchReport> CalculateStats(string riotId, int iterations = 1)
        {
            var riotAccount = await cache.GetRiotAccountDetailsAsync(riotId);
            Dictionary<string, string[]> matchHistoryIndices = [];
            var currentMatchHistory = matchHistoryIndices[riotAccount.Puuid] = await cache.GetMatchIdsAsync(riotAccount.Puuid, 0, iterations);
            var lastMatch = await cache.GetMatchInfoAsync(currentMatchHistory[0]);

            var allyTeam = new TeamStats();
            var enemyTeam = new TeamStats();

            Dictionary<string, PlayerStats> statsByPlayers = [];
            HashSet<string> processedMatches = [];

            foreach (var participant in lastMatch.Participants)
            {
                var participantTeam = participant.TeamId switch
                {
                    TeamDto.MatchTeam.Blue => allyTeam,
                    TeamDto.MatchTeam.Red => enemyTeam,
                    _ => ThrowHelper.ThrowInvalidOperationException<TeamStats>()
                };
                participantTeam.Players.Add(statsByPlayers[participant.Puuid] = new()
                {
                    PlayerName = $"{participant.RiotIdGameName}#{participant.RiotIdTagLine}",
                    ChampionName = participant.ChampionName,
                    ChampionId = participant.ChampionId,
                });
                if (!matchHistoryIndices.ContainsKey(participant.Puuid))
                {
                    matchHistoryIndices[participant.Puuid] = await cache.GetMatchIdsAsync(participant.Puuid, 1, iterations);
                }
            }

            for (int i = 0; i < iterations; i++)
            {
                foreach (var participant in lastMatch.Participants)
                {
                    var indices = matchHistoryIndices[participant.Puuid];
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
