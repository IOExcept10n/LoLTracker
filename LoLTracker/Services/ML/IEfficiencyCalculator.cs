using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoLTracker.Models.Dto;

namespace LoLTracker.Services.ML
{
    internal interface IEfficiencyCalculator
    {
        float Calculate(ParticipantDto participant, MatchDto match);
    }
}
