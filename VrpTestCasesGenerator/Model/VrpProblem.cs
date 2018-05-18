using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents Vehicle Routing Problem instance.
    /// </summary>
    public class VrpProblem
    {
        /// <summary>
        /// Gets or sets problem name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets problem comment. This contains optional information about authors, optimal solution etc.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets problem dimension. This is equal to number of clients + 1 (the 1 is the depot) 
        /// </summary>
        public int Dimension { get; set; }
        /// <summary>
        /// Gets or sets vehicle capacity.
        /// </summary>
        public int Capacity { get; set; }
        /// <summary>
        /// Gets or sets client demands. Element at index DepotIndex should be 0 (this is demand for depot which must be 0). 
        /// </summary>
        public int[] Demands { get; set; } 
        /// <summary>
        /// Gets or set index of depot. Typically is 0.
        /// </summary>
        public int DepotIndex { get; set; }
        /// <summary>
        /// Gets or sets distance matrix.
        /// </summary>
        public DistanceMatrix Distances { get; set; }
        /// <summary>
        /// Gets or sets node' coordinates.
        /// </summary>
        public Location[] Coordinates { get; set; }
    }
}
