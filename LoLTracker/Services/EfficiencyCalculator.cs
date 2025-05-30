using System.Collections.Generic;
using LoLTracker.Models.Dto;
using LoLTracker.Properties;
using LoLTracker.Services.ML;
using XGBoostSharp;

namespace LoLTracker.Services
{
    public static class EfficiencyCalculator
    {
        private static readonly Dictionary<ChampionPosition, IEfficiencyCalculator> boostersByLine = new() {
            { ChampionPosition.Top, new XGBoostParticipantEfficiencyCalculator(XGBClassifier.LoadFromByteArray(Resources.TopLaneCalculator)) },
            { ChampionPosition.Middle, new XGBoostParticipantEfficiencyCalculator(XGBClassifier.LoadFromByteArray(Resources.MidLaneCalculator)) },
            { ChampionPosition.Jungle, new XGBoostParticipantEfficiencyCalculator(XGBClassifier.LoadFromByteArray(Resources.JungleLaneCalculator)) },
            { ChampionPosition.Bottom, new XGBoostParticipantEfficiencyCalculator(XGBClassifier.LoadFromByteArray(Resources.BottomLaneCalculator)) },
            { ChampionPosition.Utility, new XGBoostParticipantEfficiencyCalculator(XGBClassifier.LoadFromByteArray(Resources.SupportLaneCalculator)) },
        };

        //private static readonly IProbabilityCalculator logReg = new LogRegWinProbabilityCalculator(new(Resources.LogReg));

        //private static readonly Dictionary<ChampionPosition, IEfficiencyCalculator> boostersByLine = new()
        //{
        //     { ChampionPosition.Top, new NaiveEfficiencyCalculator() },
        //     { ChampionPosition.Middle, new NaiveEfficiencyCalculator() },
        //     { ChampionPosition.Jungle, new NaiveEfficiencyCalculator () },
        //     { ChampionPosition.Bottom, new NaiveEfficiencyCalculator () },
        //     { ChampionPosition.Utility, new NaiveEfficiencyCalculator () },
        //};

        private static readonly IProbabilityCalculator logReg = new NaiveProbabilityCalculator();

        public static float CalculateEfficiency(ParticipantDto participant, MatchDto match)
        {
            if (boostersByLine.TryGetValue(participant.TeamPosition ?? ChampionPosition.Utility, out var booster))
            {
                return booster.Calculate(participant, match);
            }
            return 0;
        }

        public static float CalculateWinProbability(TeamStats allyTeam, TeamStats enemyTeam)
        {
            return logReg.Calculate(allyTeam, enemyTeam);
        }
    }
}