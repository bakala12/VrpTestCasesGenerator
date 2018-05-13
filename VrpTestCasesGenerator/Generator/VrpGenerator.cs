using System;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public class VrpGenerator : IVrpGenerator
    {
        private readonly IDemandGenerator _demandGenerator;

        public VrpGenerator(IDemandGenerator demandGenerator)
        {
            _demandGenerator = demandGenerator;
        }

        public VrpProblem Generate(GeneratorParameters parameters)
        {
            int[] demands = _demandGenerator.GenerateDemands(parameters.Clients);
            return new VrpProblem()
            {
                Capacity = parameters.Capacity,
                Comment = parameters.Comment,
                Demands = demands,
                DepotIndex = 0,
                Dimension = parameters.Clients+1,
                Distances = null,
                Name = parameters.ProblemName
            };
        }
    }
}
