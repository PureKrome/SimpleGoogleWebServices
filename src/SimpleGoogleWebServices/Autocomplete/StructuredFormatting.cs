using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    public class StructuredFormatting
    {
        [JsonProperty("main_text")]
        public string MainText { get; set; }

        [JsonProperty("secondary_text")]
        public string SecondaryText { get; set; }
    }
}