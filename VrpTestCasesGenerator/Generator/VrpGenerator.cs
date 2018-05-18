using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public class VrpGenerator : IVrpGenerator
    {
        private readonly IDemandGenerator _demandGenerator;
        private readonly IClientCoordsGenerator _clientCoordsGenerator;
        private readonly IDistanceMatrixGenerator _distanceMatrixGenerator;

        public VrpGenerator(IDemandGenerator demandGenerator, IClientCoordsGenerator clientCoordsGenerator, IDistanceMatrixGenerator distanceMatrixGenerator)
        {
            _demandGenerator = demandGenerator;
            _clientCoordsGenerator = clientCoordsGenerator;
            _distanceMatrixGenerator = distanceMatrixGenerator;
        }

        public async Task<VrpProblem> Generate(GeneratorParameters parameters)
        {
            int[] demands = _demandGenerator.GenerateDemands(parameters.Clients);
            List<Location> coords = new List<Location>(parameters.Clients + 1);
            coords.Add(parameters.Depot);
            coords.AddRange(await _clientCoordsGenerator.GenerateClientCoords(parameters.Clients, parameters.Streets));
            var matrix = await _distanceMatrixGenerator.GenerateDistanceMatrix(coords);

            var problem = new VrpProblem()
            {
                Capacity = parameters.Capacity,
                Comment = parameters.Comment,
                Demands = demands,
                DepotIndex = 0,
                Dimension = parameters.Clients+1,
                Distances = matrix,
                Name = parameters.ProblemName
            };
            if (parameters.IncludeCoords)
            {
                problem.Coordinates = coords.ToArray();
            }

            return problem;
        }
    }
}
