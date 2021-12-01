using Newtonsoft.Json;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfilingWithMiniprofiler.Services
{
    public class ZippopotamLocationService : ILocationService
    {
        private const string Endpoint = "https://api.zippopotam.us/{countrycode}/{postalCode}";
        private readonly IHttpClientFactory _httpClientFactory;

        public ZippopotamLocationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(float latitude, float longitude)> GetLatLng(string countryCode, string postalCode)
        {
            var fullUrl = Endpoint.Replace("{countrycode}", countryCode).Replace("{postalCode}", postalCode);
            var httpClient = _httpClientFactory.CreateClient();

            var response = await MiniProfiler.Current.Inline(() => httpClient.GetAsync(fullUrl), "Http call to Zippopotam");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var info = JsonConvert.DeserializeObject<Temperatures>(content);

            var avgLatitude = info.Places.Select(_ => _.Latitude).Select(_ => float.Parse(_)).Average();
            var avgLongitude = info.Places.Select(_ => _.Longitude).Select(_ => float.Parse(_)).Average();

            return (avgLatitude, avgLongitude);
        }

        private class Temperatures
        {
            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("country abbreviation")]
            public string CountryAbbreviation { get; set; }

            [JsonProperty("places")]
            public List<Place> Places { get; set; }
        }

        private class Place
        {
            [JsonProperty("place name")]
            public string PlaceName { get; set; }

            [JsonProperty("longitude")]
            public string Longitude { get; set; }

            [JsonProperty("latitude")]
            public string Latitude { get; set; }
        }
    }
}