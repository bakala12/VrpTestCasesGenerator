using System.Collections.Generic;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public class GeneratorParameters
    {
        public string ProblemName { get; set; }
        public string Comment { get; set; }
        public IEnumerable<string> Streets { get; set; }
        public int Clients { get; set; }
        public Location Depot { get; set; }
        public int Capacity { get; set; }
        public bool IncludeCoords { get; set; }
    }
}