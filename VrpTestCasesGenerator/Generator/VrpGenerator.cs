using System;
using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public class VrpGenerator : IVrpGenerator
    {
        private readonly IDemandGenerator _demandGenerator;
        private readonly IDistanceMatrixGenerator _distanceMatrixGenerator;

        public VrpGenerator(IDemandGenerator demandGenerator, IDistanceMatrixGenerator distanceMatrixGenerator)
        {
            _demandGenerator = demandGenerator;
            _distanceMatrixGenerator = distanceMatrixGenerator;
        }

        public async Task<VrpProblem> Generate(GeneratorParameters parameters)
        {
            int[] demands = _demandGenerator.GenerateDemands(parameters.Clients);
            var matrix = await _distanceMatrixGenerator.GenerateDistanceMatrix(parameters);
            return new VrpProblem()
            {
                Capacity = parameters.Capacity,
                Comment = parameters.Comment,
                Demands = demands,
                DepotIndex = 0,
                Dimension = parameters.Clients+1,
                Distances = matrix,
                Name = parameters.ProblemName
            };
        }
    }
}
