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
            var dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843)).Result;
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CorrectResultTest()
        {
            var dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843), false).Result;
            Assert.AreEqual(dist.Item1, 222, 1);
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

        [TestMethod]
        public void GraphHopperWithCoerctionTest()
        {
            var startLat = 52.196366;
            var startLon = 21.03214;
            var endLat = 52.196853;
            var endLon = 21.03104;
            var expectedGHDist = 27;
            var startDist = 37;
            var endDist = 51;
            var expectedCoerceDist = startDist+expectedGHDist+endDist;

            var actualGHDist = _client.GetDistance(new Location(startLat, startLon), new Location(endLat, endLon), false).Result;
            var actualCoercedGHDist = _client.GetDistance(new Location(startLat, startLon), new Location(endLat, endLon), true).Result;
            var euclideanDist = _client.CalculateSimpleDistance(startLat, startLon, endLat, endLon);

            Assert.AreEqual(expectedGHDist, actualGHDist.Item1, 1);
            Assert.AreEqual(expectedCoerceDist, actualCoercedGHDist.Item1,1);
            Assert.IsTrue(euclideanDist < actualCoercedGHDist.Item1);
        }

        [TestMethod]
        public void TestNumberOfCrossings()
        {
            var expectedCrossingsCount = 2;

            var dist = _client.GetDistance(new Location(52.226469, 20.989519), new Location(52.22752, 20.988843), false).Result;

            Assert.AreEqual(expectedCrossingsCount, dist.Item2);
        }
    }
}
