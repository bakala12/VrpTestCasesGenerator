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
        Task<DistanceMatrix> GenerateDistanceMatrix(GeneratorParameters parameters);
    }

    public class DistanceMatrixGenerator : IDistanceMatrixGenerator
    {
        private readonly NominatimClient _nominatimClient = new NominatimClient();
        private readonly GraphHopperClient _graphHopperClient = new GraphHopperClient();
        private readonly IStreetPointGenerator _streetPointGenerator;

        public DistanceMatrixGenerator(IStreetPointGenerator streetPointGenerator)
        {
            _streetPointGenerator = streetPointGenerator;
        }

        public async Task<DistanceMatrix> GenerateDistanceMatrix(GeneratorParameters parameters)
        {
            var streets = await GenerateStreets(parameters.Streets);
            var distSum = streets.Sum(s => s.Distance);
            var pointsPerStreet = streets.Select(s => (int)Math.Round(s.Distance / distSum)).ToArray();
            int diff = pointsPerStreet.Sum(p => p) - parameters.Clients;
            while (diff != 0)
            {
                if (diff > 0)
                    DecreaseMaximum(pointsPerStreet, ref diff);
                else
                    IncreaseMinimum(pointsPerStreet, ref diff);
            }
            var locations = new List<Location>();
            int i = 0;
            double stdDev = distSum / parameters.Clients / 3; //99,7% points are no further than 3 standard deviations from mean.
            foreach (var street in streets)
            {
                locations.AddRange(_streetPointGenerator.GeneratePointsFromStreet(street, i++, stdDev));
            }
            return await CreateMatrix(parameters.Depot, locations);
        }

        private void IncreaseMinimum(int[] points, ref int diff)
        {
            points[Enumerable.Range(0, points.Length).Aggregate((a, b) => (points[a] < points[b]) ? a : b)]++;
            diff++;
        }

        private void DecreaseMaximum(int[] points, ref int diff)
        {
            points[Enumerable.Range(0, points.Length).Aggregate((a, b) => (points[a] > points[b]) ? a : b)]--;
            diff--;
        }

        private async Task<List<Street>> GenerateStreets(IEnumerable<string> streetNames)
        {
            List<Street> streets = new List<Street>();
            foreach (var street in streetNames)
            {
                var points = await _nominatimClient.GetStreetPoints(street);
                var start = points[0];
                var end = points[points.Count - 1];
                var dist = await _graphHopperClient.GetDistance(start, end);
                streets.Add(new Street()
                {
                    Start = start,
                    End = end,
                    Distance = dist
                });
            }
            return streets;
        }

        private async Task<DistanceMatrix> CreateMatrix(Location depot, List<Location> clients)
        {
            DistanceMatrix matrix = new DistanceMatrix(clients.Count + 1);
            for (int i = 1; i <= clients.Count; i++)
            {
                for (int j = i + 1; j <= clients.Count; j++)
                {
                    matrix[i, j] = await _graphHopperClient.GetDistance(clients[i-1], clients[j-1]);
                    matrix[j, i] = await _graphHopperClient.GetDistance(clients[j-1], clients[i-1]);
                }
                matrix[i, 0] = await _graphHopperClient.GetDistance(clients[i-1], depot);
                matrix[0, i] = await _graphHopperClient.GetDistance(depot, clients[i-1]);
            }
            return matrix;
        }
    }
}