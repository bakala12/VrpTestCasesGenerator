using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Represents a client for GraphHopper web service.
    /// </summary>
    public interface IGraphHopperClient
    {
        /// <summary>
        /// Get the distance and number of crossings between two points on the map. This takes into account existing streets architecture.
        /// </summary>
        /// <param name="from">Source point.</param>
        /// <param name="to">Destination point.</param>
        /// <param name="withCoerce">Indicates whether to use start and end point distance correction.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        Task<Tuple<double, int>> GetDistance(Location from, Location to, bool withCoerce = true);
    }

    /// <summary>
    /// An implementation of IGraphHopperClient interface.
    /// </summary>
    public class GraphHopperClient : IGraphHopperClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _webServiceAddress;

        /// <summary>
        /// Initializes a new instance of GraphHopperClient.
        /// </summary>
        public GraphHopperClient()
        {
            _webServiceAddress = ConfigurationManager.AppSettings["GraphHopperAddress"];
        }

        private class PointsModel
        {
            public double[][] Coordinates { get; set; }
        }

        private class InstructionModel
        {
            public int Sign { get; set; }
        }

        private class SnappedWaypointsModel
        {
            public string Type { get; set; }
            public double[][] Coordinates { get; set; }
        }

        private class PathModel
        {
            public double Distance { get; set; }
            public PointsModel Points { get; set; }
            public InstructionModel[] Instructions { get; set; }
            [JsonProperty("snapped_waypoints")]
            public SnappedWaypointsModel SnappedWaypoints { get; set; }
        }

        private class ResponseModel
        {
            public PathModel[] Paths { get; set; } 
        }

        /// <summary>
        /// Get the distance and number of crossings between two points on the map. This takes into account existing streets architecture.
        /// </summary>
        /// <param name="from">Source point.</param>
        /// <param name="to">Destination point.</param>
        /// <param name="withCoerce">Indicates whether to use start and end point distance correction.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        public async Task<Tuple<double, int>> GetDistance(Location from, Location to, bool withCoerce = true)
        {
            var builder = new UriBuilder(_webServiceAddress);
            var parameters = new List<(string, string)>();
            parameters.Add(("point", from.ToString()));
            parameters.Add(("point", to.ToString()));
            parameters.Add(("locale", "pl-PL"));
            parameters.Add(("instructions", "true"));
            parameters.Add(("vehicle", "car"));
            parameters.Add(("weighting", "fastest"));
            parameters.Add(("elevation", "false"));
            parameters.Add(("points_encoded", "false"));
            parameters.Add(("use_miles", "false"));
            parameters.Add(("layer", "Omniscale"));
            builder.Query = GetQueryString(parameters);
            var response = await _client.GetAsync(builder.Uri);
            if (!response.IsSuccessStatusCode)
                throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
            var resp = JsonConvert.DeserializeObject<ResponseModel>(await response.Content.ReadAsStringAsync());
            var dist = withCoerce ? CoerceDistance(from, resp, to) : resp.Paths[0].Distance;
            return new Tuple<double, int>(dist, resp.Paths[0].Instructions.Length-2);
        }

        //This is because of GraphHopper two parameter for point key.
        private static String GetQueryString(IList<(string, string)> parameters)
        {
            StringBuilder result = new StringBuilder();
            foreach (var parameter in parameters)
            {
                result.Append(HttpUtility.UrlEncode(parameter.Item1, Encoding.UTF8));
                result.Append("=");
                result.Append(HttpUtility.UrlEncode(parameter.Item2, Encoding.UTF8));
                result.Append("&");
            }
            if (result.Length > 0)
                result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        private double CoerceDistance(Location from, ResponseModel respose, Location to)
        {
            var dist = respose.Paths[0].Distance;
            var start = respose.Paths[0].Points.Coordinates.First();
            var end = respose.Paths[0].Points.Coordinates.Last();
            return CalculateSimpleDistance(from.Latitude, from.Longitude, start[1], start[0])+dist+CalculateSimpleDistance(end[1], end[0], to.Latitude, to.Longitude);
        }

        public static readonly double EarthRadiusInMeters = 6371008.8;

        /// <summary>
        /// Calculate the distance between two points on the Earth.
        /// This function uses haversine formula. All parameters should be in degrees.
        /// </summary>
        /// <param name="startLat">Latitude of start point.</param>
        /// <param name="startLon">Longitude of start point.</param>
        /// <param name="endLat">Latitude of end point.</param>
        /// <param name="endLon">Longitude of end point.</param>
        /// <returns>Distance (in meters) between two points.</returns>
        public int CalculateSimpleDistance(double startLat, double startLon, double endLat, double endLon)
        {
            double dLat = this.toRadian(endLat - startLat);
            double dLon = this.toRadian(endLon - startLon);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(this.toRadian(startLat)) * Math.Cos(this.toRadian(endLat)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = EarthRadiusInMeters * c;
            return (int)Math.Round(d);
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
