using Newtonsoft.Json;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfilingWithMiniprofiler.Services
{
    internal class OpenMeteoWeatherService : IWeatherService
    {
        private const string Endpoint = "https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lng}&hourly=temperature_2m";
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenMeteoWeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<double> GetTemperature(float latitude, float longitude)
        {
            var fullUrl = Endpoint.Replace("{lat}", latitude.ToString()).Replace("{lng}", longitude.ToString());
            var httpClient = _httpClientFactory.CreateClient();

            var response = await MiniProfiler.Current.Inline(() => httpClient.GetAsync(fullUrl), "Http call to OpenMeteo");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var temperatureInfo = JsonConvert.DeserializeObject<Temperatures>(content);

            return temperatureInfo.Hourly.Temperature2M.Average();
        }

        private class Temperatures
        {
            [JsonProperty("latitude")]
            public double Latitude { get; set; }

            [JsonProperty("longitude")]
            public double Longitude { get; set; }

            [JsonProperty("elevation")]
            public double Elevation { get; set; }

            [JsonProperty("hourly")]
            public Hourly Hourly { get; set; }
        }

        private class Hourly
        {
            [JsonProperty("temperature_2m")]
            public List<double> Temperature2M { get; set; }
        }
    }
}