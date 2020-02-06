using System.Threading;
using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace WorldDomination.SimpleGoogleWebServices
{
    public interface IGoogleMapsApiService
    {
        Task<GeocodeResult> GeocodeAsync(GeocodeQuery query, CancellationToken cancellationToken);
    }
}