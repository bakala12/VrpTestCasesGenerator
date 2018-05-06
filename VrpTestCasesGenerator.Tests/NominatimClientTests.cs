using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VrpTestCasesGenerator.Generator;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Tests
{
    [TestClass]
    public class NominatimClientTests
    {
        private readonly NominatimClient _client = new NominatimClient();
        
        [TestMethod]
        public void PointsAreFoundForValidStreet()
        {
            var result = _client.GetStreetPoints("Noakowskiego");
            Assert.IsTrue(result.Result.Count > 0);
        }

        [TestMethod]
        public void ReverseGeocodeFindsCorrectCity()
        {
            var location = new Location
            {
                Latitude = 52.22236595,
                Longitude = 21.0090823959225
            };
            var result = _client.GetAddress(location);
            Assert.IsTrue(result.Result.City == "Warszawa");
        }
    }
}
