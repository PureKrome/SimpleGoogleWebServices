using System.Collections.Generic;
using System.Linq;

namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    public class AutocompleteResult : ApiResult
    {
        public IEnumerable<Address> Results { get; set; } = Enumerable.Empty<Address>();
    }
}