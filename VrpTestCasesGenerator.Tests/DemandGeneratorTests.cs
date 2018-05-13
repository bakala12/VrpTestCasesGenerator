using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrpTestCasesGenerator.Generator;

namespace VrpTestCasesGenerator.Tests
{
    [TestClass]
    public class DemandGeneratorTests
    {
        [TestMethod]
        public void DemeandGenerationTest()
        {
            var gen = new DemandGenerator(0.1, 1, 1000);
            var demands = gen.GenerateDemands(10);
            foreach (var demand in demands)
            {
                Assert.IsTrue(demand >0 && demand < 1000);
            }
        }
    }
}
