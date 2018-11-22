using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math.Distances;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Provides a method for generating distance matrix for VRP problem.
    /// </summary>
    public interface IDistanceMatrixGenerator
    {
        /// <summary>
        /// Generates distance matrix for the given locations.
        /// </summary>
        /// <param name="locations">List of points.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        Task<DistanceMatrix> GenerateDistanceMatrix(IList<Location> locations);
    }

    /// <summary>
    /// An implementation of IDistanceMatrixGenerator interface that make use of GraphHopper web service.
    /// </summary>
    public class DistanceMatrixGenerator : IDistanceMatrixGenerator
    {
        private readonly IGraphHopperClient _graphHopperClient;

        /// <summary>
        /// Initializes a new instance of Distance matrix generator.
        /// </summary>
        /// <param name="graphHopperClient"></param>
        public DistanceMatrixGenerator(IGraphHopperClient graphHopperClient)
        {
            _graphHopperClient = graphHopperClient;
        }

        /// <summary>
        /// Generates distance matrix for the given locations.
        /// </summary>
        /// <param name="locations">List of points.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        public async Task<DistanceMatrix> GenerateDistanceMatrix(IList<Location> locations)
        {
            DistanceMatrix matrix = new DistanceMatrix(locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                for (int j = 0; j < locations.Count; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        matrix.SetCrossingCount(i, j, 0);
                    }
                    else
                    {
                        var res = await _graphHopperClient.GetDistance(locations[i], locations[j]);
                        matrix[i, j] = res.Item1;
                        matrix.SetCrossingCount(i, j, res.Item2);
                    }
                }
            }
            return matrix;
        }
    }
}