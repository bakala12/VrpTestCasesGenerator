using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Implementation of IVrpGenerator interface.
    /// </summary>
    public class VrpGenerator : IVrpGenerator
    {
        private readonly IDemandGenerator _demandGenerator;
        private readonly IClientCoordsGenerator _clientCoordsGenerator;
        private readonly IDistanceMatrixGenerator _distanceMatrixGenerator;

        /// <summary>
        /// Initializes a new instance of VrpGenerator.
        /// </summary>
        /// <param name="demandGenerator">Demand generator.</param>
        /// <param name="clientCoordsGenerator">Client coordinates generator.</param>
        /// <param name="distanceMatrixGenerator">Distance matrix generator.</param>
        public VrpGenerator(IDemandGenerator demandGenerator, IClientCoordsGenerator clientCoordsGenerator, IDistanceMatrixGenerator distanceMatrixGenerator)
        {
            _demandGenerator = demandGenerator;
            _clientCoordsGenerator = clientCoordsGenerator;
            _distanceMatrixGenerator = distanceMatrixGenerator;
        }

        /// <summary>
        /// Generates random map VRP problem instance.
        /// </summary>
        /// <param name="parameters">Problem parameters.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        public async Task<VrpProblem> Generate(GeneratorParameters parameters)
        {
            int[] demands = _demandGenerator.GenerateDemands(parameters.Clients);
            List<Location> coords = new List<Location>(parameters.Clients + 1);
            coords.Add(parameters.Depot);
            var generated = await _clientCoordsGenerator.GenerateClientCoords(parameters.Clients, parameters.Streets);
            coords.AddRange(generated.SelectMany(x => x.Locations));
            var groups = new Dictionary<int, LocationGroup>();
            int id = 1;
            foreach (var g in generated)
            {
                foreach (var gLocation in g.Locations)
                {
                    groups.Add(id++, g.LocationGroup);
                }
            }
            var matrix = await _distanceMatrixGenerator.GenerateDistanceMatrix(coords);

            var problem = new VrpProblem()
            {
                Capacity = parameters.Capacity,
                Comment = parameters.Comment,
                Demands = demands,
                DepotIndex = 0,
                Dimension = parameters.Clients+1,
                Distances = matrix,
                Name = parameters.ProblemName,
                LocationGroups = groups
            };
            if (parameters.IncludeCoords)
            {
                problem.Coordinates = coords.ToArray();
            }

            return problem;
        }
    }
}
