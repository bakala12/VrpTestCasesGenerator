using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents a locations within the same location group.
    /// </summary>
    public class GeneratedLocations
    {
        /// <summary>
        /// Gets or sets generated locations.
        /// </summary>
        public List<Location> Locations { get; set; }
        /// <summary>
        /// Gets or sets location group.
        /// </summary>
        public LocationGroup LocationGroup { get; set; }
    }
}
