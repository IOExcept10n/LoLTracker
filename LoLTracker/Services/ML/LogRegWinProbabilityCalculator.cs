using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace LoLTracker.Services.ML
{
    internal class LogRegWinProbabilityCalculator(InferenceSession session) : IProbabilityCalculator
    {
        public float Calculate(TeamStats ally, TeamStats enemy)
        {
            float totalEfficiency = (float)(ally.TotalEfficiency + enemy.TotalEfficiency);
            float[] inputData = [
                (float)ally.TotalEfficiency / totalEfficiency,
                (float)enemy.TotalEfficiency / totalEfficiency,
            ];
            var inputTensor = new DenseTensor<float>(inputData, [1, inputData.Length]);
            NamedOnnxValue[] inputs = [NamedOnnxValue.CreateFromTensor("float_input", inputTensor)];
            using var results = session.Run(inputs);

            var output = (results[1].Value as IList<DisposableNamedOnnxValue>)?[0].AsDictionary<long, float>();
            float winProbability = 0;
            output?.TryGetValue(1, out winProbability);
            return winProbability;
        }
    }
}
