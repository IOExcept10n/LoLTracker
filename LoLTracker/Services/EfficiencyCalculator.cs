using System;
using System.Collections.Generic;
using LoLTracker.Models.Dto;

namespace LoLTracker.Services
{
    public static class EfficiencyCalculator
    {
        public static double CalculateEfficiency(ParticipantDto participant, MatchDto match)
        {
            return participant.TotalDamageDealt;
        }

        public static double CalculateWinProbability(double allyTeam, double enemyTeam)
        {
            return 1 / (1 + Math.Exp(-(allyTeam - enemyTeam)));
        }
    }
}
