using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents a part of the street.
    /// </summary>
    public struct Street
    {
        /// <summary>
        /// Gets the distance from the beginning to the end of the street.
        /// </summary>
        public double Distance { get; }

        /// <summary>
        /// Gets the name of the street.
        /// </summary>
        public string Name { get; }

        private readonly double[] _nodeDistances;
        private readonly List<Location> _streetPoints;

        /// <summary>
        /// Initializes a new instance of the street.
        /// </summary>
        /// <param name="name">Name of the street.</param>
        /// <param name="streetPoints">Street points.</param>
        public Street(string name, List<Location> streetPoints)
        {
            Name = name;
            _streetPoints = streetPoints;
            _nodeDistances = new double[streetPoints.Count - 1];

            Distance = 0;
            for (int i = 0; i < streetPoints.Count - 1; i++)
            {
                Location from = streetPoints[i];
                Location to = streetPoints[i + 1];

                _nodeDistances[i] = GetEuclideanDistance(from, to);
                Distance += _nodeDistances[i];
            }
        }

        /// <summary>
        /// Gets intermediate point of the street at the given distance.
        /// </summary>
        /// <param name="distance">Distance.</param>
        /// <returns>A point on the street at the given distance from beginning.</returns>
        public Location GetIntermediatePoint(double distance)
        {
            if (distance < 0 || distance > Distance)
                throw new ArgumentOutOfRangeException();

            int i = 0;
            for (; i < _nodeDistances.Length; i++)
            {
                if (distance > _nodeDistances[i])
                    distance -= _nodeDistances[i];
                else
                    break;
            }

            Location from = _streetPoints[i];
            Location to = _streetPoints[i + 1];
            double coeff = distance / _nodeDistances[i];

            return new Location
            {
                Latitude = from.Latitude + (to.Latitude - from.Latitude) * coeff,
                Longitude = from.Longitude + (to.Longitude - from.Longitude) * coeff
            };
        }

        private static double GetEuclideanDistance(Location a, Location b)
        {
            return Math.Sqrt(
                (a.Latitude - b.Latitude) * (a.Latitude - b.Latitude) +
                (a.Longitude - b.Longitude) * (a.Longitude - b.Longitude));
        }
    }
}
