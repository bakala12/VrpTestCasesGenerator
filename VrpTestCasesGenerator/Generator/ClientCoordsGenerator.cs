using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IClientCoordsGenerator
    {
        Task<List<Location>> GenerateClientCoords(int clientCount, IEnumerable<string> streetNames);
    }

    public class ClientCoordsGenerator : IClientCoordsGenerator
    {
        private readonly INominatimClient _nominatimClient;
        private MultivariateNormalDistribution _multivariateNormalDistribution;

        public ClientCoordsGenerator(INominatimClient nominatimClient, double standardDeviation)
        {
            _nominatimClient = nominatimClient;

            var covarianceMatrix = new double[2, 2];
            covarianceMatrix[0, 0] = covarianceMatrix[1, 1] = standardDeviation * standardDeviation;
            _multivariateNormalDistribution = new MultivariateNormalDistribution(new double[2], covarianceMatrix);
        }

        public async Task<List<Location>> GenerateClientCoords(int clientCount, IEnumerable<string> streetNames)
        {
            List<Street> streets = new List<Street>();
            double distSum = 0;

            foreach (var streetName in streetNames)
            {
                var streetPoints = await _nominatimClient.GetStreetPoints(streetName);
                var street = new Street(streetPoints);
                streets.Add(street);
                distSum += street.Distance;
            }

            var pointsPerStreet = streets.Select(s => (int)Math.Round(clientCount * s.Distance / distSum)).ToArray();
            int diff = pointsPerStreet.Sum(p => p) - clientCount;
            while (diff != 0)
            {
                if (diff > 0)
                    DecreaseMaximum(pointsPerStreet, ref diff);
                else
                    IncreaseMinimum(pointsPerStreet, ref diff);
            }

            var locations = new List<Location>(clientCount);
            for (int i = 0; i < streets.Count; i++)
            {
                var street = streets[i];
                locations.AddRange(GenerateLocationsForStreet(street, pointsPerStreet[i]));
            }

            return locations;
        }

        private List<Location> GenerateLocationsForStreet(Street street, int count)
        {
            var locations = new List<Location>(count);
            var samples = _multivariateNormalDistribution.Generate(count);
            
            double step = street.Distance / count;
            double distance = step / 2;

            for (int i = 0; i < count; i++)
            {
                var location = street.GetIntermediatePoint(distance);
                var sample = samples[i];
                location.Latitude += sample[0];
                location.Longitude += sample[1];

                locations.Add(location);
                distance += step;
            }

            return locations;
        }

        private void IncreaseMinimum(int[] points, ref int diff)
        {
            points[Enumerable.Range(0, points.Length).Aggregate((a, b) => points[a] < points[b] ? a : b)]++;
            diff++;
        }

        private void DecreaseMaximum(int[] points, ref int diff)
        {
            points[Enumerable.Range(0, points.Length).Aggregate((a, b) => points[a] > points[b] ? a : b)]--;
            diff--;
        }
    }
}
