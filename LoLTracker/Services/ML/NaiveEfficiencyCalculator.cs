using LoLTracker.Models.Dto;

namespace LoLTracker.Services.ML
{
    internal class NaiveEfficiencyCalculator : IEfficiencyCalculator
    {
        public float Calculate(ParticipantDto participant, MatchDto match)
        {
            return (participant.ChampExperience + participant.GoldEarned) * 60 / (float)participant.TimePlayed;
        }
    }
}