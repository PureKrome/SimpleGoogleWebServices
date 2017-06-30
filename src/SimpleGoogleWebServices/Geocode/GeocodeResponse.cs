using System.Collections.Generic;
using System.Linq;

namespace WorldDomination.SimpleGoogleWebServices.Geocode
{
    /// <summary>
    /// This is the internal, full API response from Google's API when geocoding.
    /// </summary>
    internal class GeocodeResponse : ApiResult
    {
        public IEnumerable<Result> Results { get; set; } = Enumerable.Empty<Result>();
    }
}