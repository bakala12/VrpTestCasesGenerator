using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrpTestCasesGenerator.Generator;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Test
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
            double dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843), false).Result;
            Assert.AreEqual(dist, 222, 1);
        }

        [TestMethod]
        public void CalculateDistanceTest()
        {
            var startLat = 52.226469;
            var startLon = 20.989519;
            var endLat = 52.22752;
            var endLon = 20.988843;
            var expectedDist = 126;

            var actualDist = _client.CalculateSimpleDistance(startLat, startLon, endLat, endLon);

            Assert.AreEqual(expectedDist, actualDist);
        }
    }
}
