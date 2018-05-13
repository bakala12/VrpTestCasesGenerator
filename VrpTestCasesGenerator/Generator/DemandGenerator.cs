using Accord.Statistics.Distributions.Univariate;

namespace VrpTestCasesGenerator.Generator
{
    public interface IDemandGenerator
    {
        int[] GenerateDemands(int clients);
    }

    public class DemandGenerator : IDemandGenerator
    {
        private readonly BetaDistribution _betaDistribution;
        private readonly int _capacity;

        public DemandGenerator(double alpha, double beta, int capacity)
        {
            _betaDistribution = new BetaDistribution(alpha, beta);
            _capacity = capacity;
        }

        public int[] GenerateDemands(int clients)
        {
            var demands = new int[clients];
            for (int i = 0; i < clients; i++)
            {
                demands[i] = (int)(1+_betaDistribution.Generate() * (_capacity-1)); //to have result from 1 to capacity
            }
            return demands;
        }
    }
}