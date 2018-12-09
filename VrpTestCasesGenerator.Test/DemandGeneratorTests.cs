using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrpTestCasesGenerator.Generator;

namespace VrpTestCasesGenerator.Test
{
    [TestClass]
    public class DemandGeneratorTests
    {
        [TestMethod]
        public void DemandGenerationTest()
        {
            var gen = new BetaDemandGenerator(0.1, 1, 1000);
            var demands = gen.GenerateDemands(10);
            foreach (var demand in demands)
            {
                Assert.IsTrue(demand > 0 && demand < 1000);
            }
        }

        [TestMethod]
        public void GammaDemandTests()
        {
            var gen = new GammaDemandGenerator(1.9, 0.033, 1000);
            var demands = gen.GenerateDemands(1000);
            foreach (var demand in demands)
            {
                Assert.IsTrue(demand > 0 && demand < 1000);
            }
        }
    }
}
