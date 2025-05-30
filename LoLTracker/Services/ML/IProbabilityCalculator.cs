using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTracker.Services.ML
{
    internal interface IProbabilityCalculator
    {
        float Calculate(TeamStats ally, TeamStats enemy);
    }
}
