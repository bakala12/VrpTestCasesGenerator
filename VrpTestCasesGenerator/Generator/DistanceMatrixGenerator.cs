using System.Collections;
using System.Collections.Generic;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IDistanceMatrixGenerator
    {
        DistanceMatrix GenerateDistanceMatrix(GeneratorParameters parameters);
    }

    public class DistanceMatrixGenerator : IDistanceMatrixGenerator
    {
        private readonly NominatimClient _nominatimClient = new NominatimClient();
        private readonly GraphHopperClient _graphHopperClient = new GraphHopperClient();

        public DistanceMatrix GenerateDistanceMatrix(GeneratorParameters parameters)
        {
            return null;
        }
    }
}