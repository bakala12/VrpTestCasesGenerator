using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Provides a method for generating client location at given streets.
    /// </summary>
    public interface IClientCoordsGenerator
    {
        /// <summary>
        /// Generates random client location on the map using given list of streets.
        /// </summary>
        /// <param name="clientCount"></param>
        /// <param name="streetNames">List of street names.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        Task<List<GeneratedLocations>> GenerateClientCoords(int clientCount, IEnumerable<string> streetNames);
    }

    /// <summary>
    /// An implementation of IClientCoordsGenerator interface. It uses Nominatim web service to get information
    /// about the streets' geometry and then use two dimensional uniform distribution to generate random points
    /// next to the streets.
    /// </summary>
    public class ClientCoordsGenerator : IClientCoordsGenerator
    {
        private readonly INominatimClient _nominatimClient;
        private readonly Independent<UniformContinuousDistribution> _distribution;
        private const double dist = 0.0005396; //constant that is approximatively 60m in Earth coordinate

        /// <summary>
        /// Initializes a new instance of ClientCoordsGenerator class.
        /// </summary>
        /// <param name="nominatimClient">Nominatim client.</param>
        /// <param name="standardDeviation">Standard deviation for two dimensional uniform distribution.</param>
        public ClientCoordsGenerator(INominatimClient nominatimClient, double standardDeviation)
        {
            _nominatimClient = nominatimClient;
            var uniform = new UniformContinuousDistribution(-dist, +dist);
            _distribution = new Independent<UniformContinuousDistribution>(uniform, uniform);
        }

        /// <summary>
        /// Generates random client location on the map using given list of streets.
        /// </summary>
        /// <param name="clientCount"></param>
        /// <param name="streetNames">List of street names.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        public async Task<List<GeneratedLocations>> GenerateClientCoords(int clientCount, IEnumerable<string> streetNames)
        {
            List<Street> streets = new List<Street>();
            double distSum = 0;
            var streetDic = new Dictionary<string, int>();
            int sid = 1;
            foreach (var streetName in streetNames)
            {
                var streetParts = await _nominatimClient.GetStreetParts(streetName);
                if(streetParts.Count == 0)
                    Console.WriteLine($"No street parts for street {streetName}");
                var realStreet = streetParts.Select(s => new Street(streetName, s));
                streets.AddRange(realStreet);
                distSum += realStreet.Sum(s => s.Distance);
                if(!streetDic.ContainsKey(streetName))
                    streetDic.Add(streetName, sid++);
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

            var locations = new List<GeneratedLocations>();
            int id = 0;
            for (int i = 0; i < streets.Count; i++)
            {
                var street = streets[i];
                var loc = GenerateLocationsForStreet(street, pointsPerStreet[i]);
                if (!loc.Any()) continue;
                var group = new LocationGroup()
                {
                    StreetId = streetDic[street.Name],
                    StreetPartId = ++id,
                    StreetName = street.Name
                };
                var generated = new GeneratedLocations()
                {
                    LocationGroup = group,
                    Locations = loc
                };
                locations.Add(generated);
            }
            return locations;
        }

        private List<Location> GenerateLocationsForStreet(Street street, int count)
        {
            var locations = new List<Location>(count);
            var samples = _distribution.Generate(count);

            double step = street.Distance / count;
            double distance = step / 2;

            for (int i = 0; i < count; i++)
            {
                var location = street.GetIntermediatePoint(distance); //sprawdzic
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
