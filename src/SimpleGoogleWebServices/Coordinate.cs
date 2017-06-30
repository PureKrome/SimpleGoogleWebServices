using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices
{
    public class Coordinate
    {
        [JsonProperty("lat")]
        public decimal Latitude { get; set; }
        [JsonProperty("lng")]
        public decimal Longitude { get; set; }

        public override string ToString()
        {
            return $"Lat:{Latitude} Long:{Longitude}";
        }
    }
}