using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices
{
    public abstract class ApiResult
    {
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}