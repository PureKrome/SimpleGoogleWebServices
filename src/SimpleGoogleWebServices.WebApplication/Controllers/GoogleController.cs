using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorldDomination.SimpleGoogleWebServices;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using WorldDomination.SimpleGoogleWebServices.Geocode;

// ReSharper disable ConsiderUsingConfigureAwait

namespace SimpleGoogleWebServices.WebApplication.Controllers
{
    [Route("google")]
    public class GoogleController
    {
        [HttpGet]
        [Route("geocode")]
        public async Task<IActionResult> GeocodeAsync(string address, string postcode, string key, CancellationToken cancellationToken)
        {
            var service = new GoogleMapsApiService(key);
            ComponentFilters filters = null;
            if (!string.IsNullOrWhiteSpace(postcode))
            {
                filters = new ComponentFilters {PostalCode = postcode};
            }
            var geocodeQuery = new GeocodeQuery
            {
                Address = address,
                ComponentFilters = filters
            };
            var response = await service.GeocodeAsync(geocodeQuery, cancellationToken);

            return new JsonResult(response);
        }

        [HttpGet]
        [Route("cleanup")]
        public async Task<IActionResult> CleanupAsync(string address, string key, CancellationToken cancellationToken)
        {
            var service = new GooglePlacesApiService(key);
            var autocompleteQuery = new AutocompleteQuery
            {
                AutocompleteType = AutocompleteType.Address,
                Query = address
            };
            var response = await service.CleanUpAddressAsync(autocompleteQuery, cancellationToken);
            return new JsonResult(response);
        }
    }
}