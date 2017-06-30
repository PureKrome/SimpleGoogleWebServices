using System.Collections.Generic;
using System.Linq;

namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    public class AutocompleteResponse : ApiResult
    {
        public IEnumerable<Prediction> Predictions { get; set; } = Enumerable.Empty<Prediction>();
    }
}