using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrpTestCasesGenerator.Model
{
    public struct Street
    {
        public Location Start { get; set; }
        public Location End { get; set; }
        public double Distance { get; set; }
    }
}
