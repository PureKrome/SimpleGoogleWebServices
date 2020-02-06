using System.Threading;
using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using WorldDomination.SimpleGoogleWebServices.Details;

namespace WorldDomination.SimpleGoogleWebServices
{
    public interface IGooglePlacesApiService
    {
        Task<AutocompleteResult> AutocompleteAsync(AutocompleteQuery query, CancellationToken cancellationToken);
        Task<DetailsResult> DetailsAsync(DetailsQuery query, CancellationToken cancellationToken);
        Task<DetailsResult> CleanUpAddressAsync(AutocompleteQuery query, CancellationToken cancellationToken);
    }
}