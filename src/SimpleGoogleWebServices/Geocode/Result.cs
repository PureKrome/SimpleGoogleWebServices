using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices.Geocode
{
    internal class Result
    {
        [JsonProperty("address_components")]
        public List<AddressComponent> AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        public Geometry Geometry { get; set; }

        [JsonProperty("partial_match")]
        public bool PartialMatch { get; set; }

        public List<string> Types { get; set; }

        public Coordinate ToCoordinate => Geometry?.Location;
    }
}