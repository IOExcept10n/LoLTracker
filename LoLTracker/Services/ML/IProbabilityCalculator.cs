namespace LoLTracker.Services.ML
{
    internal interface IProbabilityCalculator
    {
        float Calculate(TeamStats ally, TeamStats enemy);
    }
}