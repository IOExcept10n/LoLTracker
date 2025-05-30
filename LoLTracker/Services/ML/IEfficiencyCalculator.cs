using LoLTracker.Models.Dto;

namespace LoLTracker.Services.ML
{
    internal interface IEfficiencyCalculator
    {
        float Calculate(ParticipantDto participant, MatchDto match);
    }
}