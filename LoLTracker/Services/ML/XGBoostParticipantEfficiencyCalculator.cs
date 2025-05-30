using LoLTracker.Models.Dto;
using XGBoostSharp;
using XGBoostSharp.Lib;

namespace LoLTracker.Services.ML
{
    internal class XGBoostParticipantEfficiencyCalculator(XGBClassifier classifier) : IEfficiencyCalculator
    {
        public float Calculate(ParticipantDto participant, MatchDto match)
        {
            float[] data = [
                participant.Deaths,
                participant.GoldEarned,
                participant.Kills,
                participant.Assists,
                participant.ChampExperience,
                participant.TotalDamageDealtToChampions,
                participant.VisionScore,
                participant.TotalMinionsKilled,
                participant.DamageSelfMitigated,
                participant.TotalHeal,
                participant.TurretTakedowns,
                participant.DragonKills,
                participant.BaronKills,
                participant.WardsPlaced,
                participant.WardsKilled,
            ];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 60;
                data[i] /= participant.TimePlayed;
            }
            DMatrix matrix = new(data, 1, (ulong)data.Length);
            var result = classifier.PredictProbability(matrix);
            return result[0][1] * 1000;
        }
    }
}
