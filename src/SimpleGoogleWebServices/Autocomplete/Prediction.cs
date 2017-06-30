using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    public class Prediction
    {
        public string Description { get; set; }
        public string Id { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        public string Reference { get; set; }

        [JsonProperty("structured_formatting")]
        public StructuredFormatting StructuredFormatting { get; set; }

        public List<Term> Terms { get; set; }
        public List<string> Types { get; set; }
    }
}