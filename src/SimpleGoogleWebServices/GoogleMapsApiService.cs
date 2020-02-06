using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace WorldDomination.SimpleGoogleWebServices
{
    public class GoogleMapsApiService : IGoogleMapsApiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GoogleMapsApiService() : this(null, null)
        {
        }

        public GoogleMapsApiService(string apiKey) : this(apiKey, null)
        {
        }
        
        public GoogleMapsApiService(string apiKey, HttpClient httpClient)
        {
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _apiKey = apiKey.Trim();
            }
            
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<GeocodeResult> GeocodeAsync(GeocodeQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (string.IsNullOrWhiteSpace(query.Address))
            {
                throw new ArgumentNullException(nameof(query.Address));
            }

            var address = Uri.EscapeDataString(query.Address);
            var requestUrl = new StringBuilder();
            requestUrl.Append($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&sensor=false");

            // Append the API key if it's been provided.
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                requestUrl.Append($"&key={_apiKey}");
            }

            if (query.ComponentFilters != null)
            {
                var components = ConvertCompenentFiltersToQuerystringParameter(query.ComponentFilters);
                if (!string.IsNullOrWhiteSpace(components))
                {
                    requestUrl.Append($"&components={components}");
                }
            }

            var response = await _httpClient.GetAsync(requestUrl.ToString(), cancellationToken)
                                            .ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync()
                                        .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage =
                    $"Failed to retrieve a Google Maps Geocode result. Status Code: {response.StatusCode}. Message: {content}";
                throw new Exception(errorMessage);
            }

            // Convert the response JSON content into a nice rich object model.
            var geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(content);

            // Now project the address and lat/long data.
            var locations = from g in geocodeResponse.Results
                            select new Location
                            {
                                Id = query.Id,
                                Address = g.FormattedAddress,
                                Coordinate = g.ToCoordinate
                            };

            // Package this up into a nice result object.
            return new GeocodeResult
            {
                Status = geocodeResponse.Status,
                ErrorMessage = geocodeResponse.ErrorMessage,
                Results = locations
            };
        }

        // REF: https://developers.google.com/maps/documentation/geocoding/#ComponentFiltering
        private static string ConvertCompenentFiltersToQuerystringParameter(ComponentFilters filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var items = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filters.Route))
            {
                items.Add("route", filters.Route);
            }

            if (!string.IsNullOrWhiteSpace(filters.Locality))
            {
                items.Add("locality", filters.Locality);
            }

            if (!string.IsNullOrWhiteSpace(filters.AdministrativeArea))
            {
                items.Add("administrative_area", filters.AdministrativeArea);
            }

            if (!string.IsNullOrWhiteSpace(filters.PostalCode))
            {
                items.Add("postal_code", filters.PostalCode);
            }

            if (!string.IsNullOrWhiteSpace(filters.CountryCodeIso))
            {
                items.Add("country", filters.CountryCodeIso);
            }

            if (!items.Any())
            {
                return string.Empty;
            }

            var queryString = new StringBuilder();
            foreach (var item in items)
            {
                if (queryString.Length > 0)
                {
                    queryString.Append("|");
                }

                queryString.AppendFormat("{0}:{1}", item.Key, item.Value);
            }

            return queryString.ToString();
        }
    }
}