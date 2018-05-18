using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrpTestCasesGenerator.Model
{
    public struct Street
    {
        public double Distance { get; }

        private readonly double[] _nodeDistances;
        private readonly List<Location> _streetPoints;

        public Street(List<Location> streetPoints)
        {
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
