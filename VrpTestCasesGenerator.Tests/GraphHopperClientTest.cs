using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrpTestCasesGenerator.Generator;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Tests
{
    [TestClass]
    public class GraphHopperClientTest
    {
        private readonly GraphHopperClient _client = new GraphHopperClient();

        [TestMethod]
        public void ConnectivityTest()
        {
            double dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843)).Result;
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CorrectResultTest()
        {
            double dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843)).Result;
            Assert.AreEqual(dist, 220, 1);
        }
    }
}
