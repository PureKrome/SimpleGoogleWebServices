using Newtonsoft.Json;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace WorldDomination.SimpleGoogleWebServices
{
    internal class Geometry
    {
        public Coordinate Location { get; set; }

        public Bounds Bounds { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        public Viewport Viewport { get; set; }
    }
}