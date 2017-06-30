using System.Collections.Generic;
using System.Linq;

namespace WorldDomination.SimpleGoogleWebServices.Geocode
{
    public class GeocodeResult : ApiResult
    {
        public IEnumerable<Location> Results { get; set; } = Enumerable.Empty<Location>();
    }
}