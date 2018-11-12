using Accord.Statistics.Distributions.Univariate;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Provides a way for generating random demand array.
    /// </summary>
    public interface IDemandGenerator
    {
        /// <summary>
        /// Generates random demand array.
        /// </summary>
        /// <param name="clients">Number of clients.</param>
        /// <returns>Demand array.</returns>
        int[] GenerateDemands(int clients);
    }

    /// <summary>
    /// IDemandGenerator interface implementation that uses beta distribution.
    /// </summary>
    public class DemandGenerator : IDemandGenerator
    {
        private readonly BetaDistribution _betaDistribution;
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of DemandGenerator.
        /// </summary>
        /// <param name="alpha">Alpha parameter for beta distribution.</param>
        /// <param name="beta">Beta parameter for beta distribution.</param>
        /// <param name="capacity">Max value of generated demand.</param>
        public DemandGenerator(double alpha, double beta, int capacity)
        {
            _betaDistribution = new BetaDistribution(alpha, beta);
            _capacity = capacity;
        }

        /// <summary>
        /// Generates random demand array.
        /// </summary>
        /// <param name="clients">Number of clients.</param>
        /// <returns>Demand array.</returns>
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