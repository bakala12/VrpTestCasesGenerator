using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Fields.Features;

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
    public class BetaDemandGenerator : IDemandGenerator
    {
        private readonly BetaDistribution _betaDistribution;
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of BetaDemandGenerator.
        /// </summary>
        /// <param name="alpha">Alpha parameter for beta distribution.</param>
        /// <param name="beta">Beta parameter for beta distribution.</param>
        /// <param name="capacity">Max value of generated demand.</param>
        public BetaDemandGenerator(double alpha, double beta, int capacity)
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
                var r = _betaDistribution.Generate();
                demands[i] = (int)(1+ r * (_capacity-1)); //to have result from 1 to capacity
            }
            return demands;
        }
    }

    /// <summary>
    /// IDemandGenerator interface implementation that uses beta distribution.
    /// </summary>
    public class GammaDemandGenerator : IDemandGenerator
    {
        private readonly GammaDistribution _gammaDistribution;
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of GammaDemandGenerator.
        /// </summary>
        /// <param name="shape">Shape parameter for gamma distribution.</param>
        /// <param name="rate">Rate parameter for gamma distribution.</param>
        /// <param name="capacity">Max value of generated demand.</param>
        public GammaDemandGenerator(double shape, double rate, int capacity)
        {
            _gammaDistribution = new GammaDistribution(rate, shape);
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
                var r = _gammaDistribution.Generate();
                demands[i] = (int)(1 + r * (_capacity - 1)); //to have result from 1 to capacity
                if (demands[i] > _capacity)
                {
                    demands[i] = 1+ (demands[i] % (_capacity -1));
                }
            }
            return demands;
        }
    }
}