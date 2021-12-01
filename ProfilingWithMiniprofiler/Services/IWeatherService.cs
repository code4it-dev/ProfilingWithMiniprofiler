using System.Threading.Tasks;

namespace ProfilingWithMiniprofiler.Services
{
    public interface IWeatherService
    {
        Task<double> GetTemperature(float latitude, float longitude);
    }
}