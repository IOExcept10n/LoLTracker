using System;

namespace LoLTracker.Services.ML
{
    internal class NaiveProbabilityCalculator : IProbabilityCalculator
    {
        public float Calculate(TeamStats ally, TeamStats enemy)
        {
            double normalizedDiff = (ally.TotalEfficiency - enemy.TotalEfficiency) / (ally.TotalEfficiency + enemy.TotalEfficiency);
            return (float)(1 / (1 + Math.Exp(-5 * normalizedDiff)));
        }
    }
}