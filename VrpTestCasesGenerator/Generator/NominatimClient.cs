﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public class NominatimClient
    {
        private readonly HttpClient _httpClient;

        public NominatimClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "VrpTestCasesGenerator");
        }

        public async Task<List<Location>> GetStreetPoints(string streetName)
        {
            var builder = new UriBuilder(@"https://nominatim.openstreetmap.org/search");

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["format"] = "xml";
            query["polygon_kml"] = "1";
            query["city"] = "Warszawa";
            query["street"] = streetName;
            builder.Query = query.ToString();

            var response = await GetAsync(builder.Uri);
            if (!response.IsSuccessStatusCode)
                throw new HttpException((int)response.StatusCode, response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            XDocument xmlContent;
            using (var reader = new StringReader(content))
                xmlContent = XDocument.Load(reader);
            
            var placeElement = xmlContent
                .Element("searchresults")
                .Elements("place")
                .Where(e => 
                    e.Attribute("osm_type").Value == "way")
                .OrderByDescending(f =>
                    double.Parse(
                        f.Attribute("importance").Value, 
                        CultureInfo.InvariantCulture))
                .FirstOrDefault();
            
            if (placeElement == default(XElement))
                return new List<Location>(); // The request yielded no results.

            var coordString = placeElement
                .Element("geokml")
                .Element("LineString")
                .Element("coordinates")
                .Value;

            var result = new List<Location>();
            foreach (var lonLatString in coordString.Split(' '))
            {
                string[] rawCoord = lonLatString.Split(',');
                var coord = new Location
                {
                    Longitude = double.Parse(rawCoord[0], CultureInfo.InvariantCulture),
                    Latitude = double.Parse(rawCoord[1], CultureInfo.InvariantCulture)
                };
                result.Add(coord);
            }
            
            return result;
        }

        public async Task<Address> GetAddress(Location coords)
        {
            var builder = new UriBuilder(@"https://nominatim.openstreetmap.org/reverse");

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["lat"] = coords.Latitude.ToString(CultureInfo.InvariantCulture);
            query["lon"] = coords.Longitude.ToString(CultureInfo.InvariantCulture);
            builder.Query = query.ToString();
            
            var response = await GetAsync(builder.Uri);
            if (!response.IsSuccessStatusCode)
                throw new HttpException((int)response.StatusCode, response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            XDocument xmlContent;
            using (var reader = new StringReader(content))
                xmlContent = XDocument.Load(reader);

            var addressElement = xmlContent
                .Element("reversegeocode")
                .Element("addressparts");

            var serializer = new XmlSerializer(typeof(Address));
            using (var reader = new StringReader(addressElement.ToString()))
            {
                Address result = (Address)serializer.Deserialize(reader);
                return result;
            }
        }

        private async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            //TODO throttling (one request per second)
            return await _httpClient.GetAsync(requestUri);
        }
    }
}
