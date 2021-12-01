using Microsoft.AspNetCore.Mvc;
using ProfilingWithMiniprofiler.Services;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace ProfilingWithMiniprofiler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILocationService _locationService;

        public WeatherController(IWeatherService weatherService, ILocationService locationService)
        {
            _weatherService = weatherService;
            _locationService = locationService;
        }

        /// <summary>
        /// Get average temperature (in Celsius)
        /// </summary>
        /// <param name="countryCode">2-letters country code. Eg: IT</param>
        /// <param name="postalCode">Postal code. Eg: 10121</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<double> Get(string countryCode, string postalCode)
        {
            float latitude = 0f;//41.8955f;
            float longitude = 0f;// 12.4823f;
            double temperature = 0f;

            //string countryCode = "IT"; string postalCode = "10121";

            using (MiniProfiler.Current.Step("Get temperature for specified location"))
            {
                using (MiniProfiler.Current.Step("Getting lat-lng info"))
                {
                    (latitude, longitude) = await _locationService.GetLatLng(countryCode, postalCode);
                }

                using (MiniProfiler.Current.Step("Getting temperature info"))
                {
                    temperature = await _weatherService.GetTemperature(latitude, longitude);
                }
            }
            return temperature;
        }
    }
}