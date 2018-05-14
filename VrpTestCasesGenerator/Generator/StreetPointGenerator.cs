using System.Collections.Generic;
using System.Xml;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IStreetPointGenerator
    {
        List<Location> GeneratePointsFromStreet(Street street, int pointNumber, double standardDeviation);
    }

    public class StreetPointGenerator : IStreetPointGenerator
    {
        public List<Location> GeneratePointsFromStreet(Street street, int pointNumber, double standardDeviation)
        {
            var covarianceMatrix = new double[2,2];
            covarianceMatrix[0, 0] = covarianceMatrix[1, 1] = standardDeviation * standardDeviation;
            MultivariateNormalDistribution multivariateNormalDistribution = new MultivariateNormalDistribution(new double[2], covarianceMatrix);
            var samples = multivariateNormalDistribution.Generate(pointNumber);
            List<Location> points = new List<Location>();
            var diffX = (street.End.Longitude - street.Start.Longitude)/(pointNumber+1);
            var diffY = (street.End.Latitude - street.Start.Latitude)/(pointNumber+1);
            for (int i = 1; i <= pointNumber; i++)
            {
                var sample = samples[i - 1];
                var x = street.Start.Longitude + i * diffX;
                var y = street.Start.Latitude + i * diffY;
                points.Add(new Location(y+sample[1], x+sample[0]));
            }
            return points;
        }
    }
}