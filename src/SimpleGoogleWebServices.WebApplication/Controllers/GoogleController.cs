using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorldDomination.SimpleGoogleWebServices;

// ReSharper disable ConsiderUsingConfigureAwait

namespace SimpleGoogleWebServices.WebApplication.Controllers
{
    [Route("google")]
    public class GoogleController
    {
        [HttpGet]
        [Route("geocode")]
        public async Task<IActionResult> GeocodeAsync(string address, string postcode, string key)
        {
            var service = new GoogleMapsApiService(key);
            ComponentFilters filters = null;
            if (!string.IsNullOrWhiteSpace(postcode))
            {
                filters = new ComponentFilters {PostalCode = postcode};
            }

            var response = await service.GeocodeAsync(address, filters);

            return new JsonResult(response);
        }

        [HttpGet]
        [Route("cleanup")]
        public async Task<IActionResult> CleanupAsync(string address, string key)
        {
            var service = new GooglePlacesApiService(key);
            var response = await service.CleanUpAddressAsync(address);
            return new JsonResult(response);
        }
    }
}