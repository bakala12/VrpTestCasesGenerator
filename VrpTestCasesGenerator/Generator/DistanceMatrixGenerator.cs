using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math.Distances;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IDistanceMatrixGenerator
    {
        Task<DistanceMatrix> GenerateDistanceMatrix(IList<Location> locations);
    }

    public class DistanceMatrixGenerator : IDistanceMatrixGenerator
    {
        private readonly IGraphHopperClient _graphHopperClient;

        public DistanceMatrixGenerator(IGraphHopperClient graphHopperClient)
        {
            _graphHopperClient = graphHopperClient;
        }

        public async Task<DistanceMatrix> GenerateDistanceMatrix(IList<Location> locations)
        {
            DistanceMatrix matrix = new DistanceMatrix(locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                for (int j = 0; j < locations.Count; j++)
                {
                    if (i == j)
                        matrix[i, j] = 0;
                    else
                        matrix[i, j] = await _graphHopperClient.GetDistance(locations[i], locations[j]);
                }
            }
            return matrix;
        }
    }
}