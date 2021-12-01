using System.Threading.Tasks;

namespace ProfilingWithMiniprofiler.Services
{
    public interface ILocationService
    {
        Task<(float latitude, float longitude)> GetLatLng(string countryCode, string postalCode);
    }
}