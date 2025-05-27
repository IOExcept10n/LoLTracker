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
                    var trackedMatch = await cache.GetMatchInfoAsync(matchHistoryIndices[participant.Puuid][i]);
                    foreach (var champion in trackedMatch.Participants)
                    {
                        var efficiency = EfficiencyCalculator.CalculateEfficiency(participant, trackedMatch);
                        if (statsByPlayers.TryGetValue(champion.Puuid, out var stats))
                        {
                            stats.Efficiency += efficiency;
                        }
                    }
                }
            }

            allyTeam.TotalEfficiency = allyTeam.Players.Average(x => x.Efficiency);
            enemyTeam.TotalEfficiency = enemyTeam.Players.Average(x => x.Efficiency);

            return new(allyTeam, enemyTeam)
            {
                WinProbability = EfficiencyCalculator.CalculateWinProbability(allyTeam.TotalEfficiency, enemyTeam.TotalEfficiency)
            };
        }
    }
}
