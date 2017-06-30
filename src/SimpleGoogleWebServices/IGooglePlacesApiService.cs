using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using WorldDomination.SimpleGoogleWebServices.Details;

namespace WorldDomination.SimpleGoogleWebServices
{
    public interface IGooglePlacesApiService
    {
        Task<AutocompleteResult> AutocompleteAsync(string query);
        Task<DetailsResult> DetailsAsync(string placeId);
        Task<DetailsResult> CleanUpAddressAsync(string query);
    }
}