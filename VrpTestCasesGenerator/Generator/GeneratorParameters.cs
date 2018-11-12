using System.Collections.Generic;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Parameter of created VRP problem instance.
    /// </summary>
    public class GeneratorParameters
    {
        /// <summary>
        /// Gets or sets the name of output problem.
        /// </summary>
        public string ProblemName { get; set; }
        /// <summary>
        /// Gets or sets the comment for the generated problem.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets the set of street based on which instance will be generated.
        /// </summary>
        public IEnumerable<string> Streets { get; set; }
        /// <summary>
        /// Gets or sets number of clients.
        /// </summary>
        public int Clients { get; set; }
        /// <summary>
        /// Gets or sets depot location.
        /// </summary>
        public Location Depot { get; set; }
        /// <summary>
        /// Gets or sets vehicle capacity.
        /// </summary>
        public int Capacity { get; set; }
        /// <summary>
        /// Indicates whether to include coordinates of the given points in the problem description file.
        /// </summary>
        public bool IncludeCoords { get; set; }
    }
}