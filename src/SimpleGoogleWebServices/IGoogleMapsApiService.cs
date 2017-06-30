using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace WorldDomination.SimpleGoogleWebServices
{
    public interface IGoogleMapsApiService
    {
        Task<GeocodeResult> GeocodeAsync(string query, ComponentFilters filters = null);
    }
}