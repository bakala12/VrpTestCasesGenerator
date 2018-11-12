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
        /// Get the distance between two points on the map. This takes into account existing streets architecture.
        /// </summary>
        /// <param name="from">Source point.</param>
        /// <param name="to">Destination point.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        Task<double> GetDistance(Location from, Location to);
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

        private class PathModel
        {
            public double Distance { get; set; }
        }

        private class ResponseModel
        {
            public PathModel[] Paths { get; set; } 
        }

        /// <summary>
        /// Get the distance between two points on the map. This takes into account existing streets architecture.
        /// </summary>
        /// <param name="from">Source point.</param>
        /// <param name="to">Destination point.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        public async Task<double> GetDistance(Location from, Location to)
        {
            var builder = new UriBuilder(_webServiceAddress);
            var parameters = new List<(string, string)>();
            parameters.Add(("point", from.ToString()));
            parameters.Add(("point", to.ToString()));
            parameters.Add(("locale", "pl-PL"));
            parameters.Add(("instructions", "false"));
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
            return resp.Paths[0].Distance;
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
    }
}
