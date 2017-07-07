using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using WorldDomination.SimpleGoogleWebServices.Details;

namespace WorldDomination.SimpleGoogleWebServices
{
    public class GooglePlacesApiService : IGooglePlacesApiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GooglePlacesApiService(string apiKey) : this(apiKey, null)
        {
        }

        public GooglePlacesApiService(string apiKey, HttpClient httpClient)
        {
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _apiKey = apiKey.Trim();
            }

            _httpClient = httpClient?? new HttpClient();
        }

        public async Task<AutocompleteResult> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            var address = Uri.EscapeDataString(query);
            var requestUrl = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={address}&types=address&key={_apiKey}";

            var httpClient = _httpClient ?? new HttpClient();
            var response = await httpClient.GetAsync(requestUrl)
                                           .ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync()
                                        .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage =
                    $"Failed to retrieve a Google Maps Autocomplete result. Status Code: {response.StatusCode}. Message: {content}";
                return new AutocompleteResult
                {
                    Status = response.StatusCode.ToString(),
                    ErrorMessage = errorMessage,
                    Results = Enumerable.Empty<Autocomplete.Address>()
                };
            }

            // Get content from json into rich object model.
            var predictions = JsonConvert.DeserializeObject<AutocompleteResponse>(content);

            // Convert / project to an Address.
            var addresses = from p in predictions.Predictions
                                   select new Autocomplete.Address
                                   {
                                       PlaceId = p.PlaceId,
                                       Location = p.Description
                                   };

            return new AutocompleteResult
            {
                Status = predictions.Status,
                ErrorMessage = predictions.ErrorMessage,
                Results = addresses
            };
        }

        public async Task<DetailsResult> DetailsAsync(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                throw new ArgumentNullException(nameof(placeId));
            }
            
            var requestUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={_apiKey}";
            
            var response = await _httpClient.GetAsync(requestUrl)
                                           .ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync()
                                        .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage =
                    $"Failed to retrieve a Google Places Details result. Status Code: {response.StatusCode}. Message: {content}";
                return new DetailsResult
                {
                    Status = response.StatusCode.ToString(),
                    ErrorMessage = errorMessage
                };
            }
            
            // Get content from json into rich object model.
            var detailsResponse = JsonConvert.DeserializeObject<DetailsResponse>(content);

            return new DetailsResult
            {
                Status = detailsResponse.Status,
                ErrorMessage = detailsResponse.ErrorMessage,
                Address = detailsResponse.Result?.ToAddress
            };
        }

        public async Task<DetailsResult> CleanUpAddressAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            // First we need to get the autocomplete results.
            var autocompleteResults = await AutocompleteAsync(query).ConfigureAwait(false);

            if (autocompleteResults.Status != "OK")
            {
                return new DetailsResult
                {
                    Status = autocompleteResults.Status,
                    ErrorMessage = autocompleteResults.ErrorMessage
                };
            }

            // Now we do a 2nd call, only if there is eactly one result.
            if (autocompleteResults.Results == null ||
                !autocompleteResults.Results.Any() ||
                autocompleteResults.Results.Count() != 1)
            {
                return new DetailsResult
                {
                    Status = "ZERO_OR_MORE_THAN_ONE_RESULT",
                    ErrorMessage =
                        "To clean up a result, we expect exactly one result. Instead we received 0 or more than 1 result. Not sure what to clean up, then."
                };
            }

            var autoCompleteResult = autocompleteResults.Results.First();
            if (string.IsNullOrWhiteSpace(autoCompleteResult.PlaceId))
            {
                return new DetailsResult
                {
                    Status = "INVALID_PLACE_ID",
                    ErrorMessage = "The placeId was missing - which is crazy because we actually have a location."
                };
            }

            return await DetailsAsync(autoCompleteResult.PlaceId).ConfigureAwait(false);
        }
    }
}
