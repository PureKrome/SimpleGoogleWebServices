using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using WorldDomination.SimpleGoogleWebServices.Details;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace SimpleGoogleWebServices.ConsoleApp
{
    class Program
    {
        private const string GoogleApiKey = "-to be set-";

        static async Task Main()
        {
            IList<CsvAddress> csvAddresses;
            const string fileDelimiter = "\t";

            var cancellationToken = new CancellationToken();

            // Read in a csv file.
            using (var reader = new StreamReader("c:\\temp\\vic.tsv"))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                    csv.Configuration.Delimiter = fileDelimiter; // Tab separated.
                    csvAddresses = csv.GetRecords<CsvAddress>().ToList();
                }
            }

            var addressResults = await ProcessAddressesAsync(csvAddresses, cancellationToken);

            var placesResults = await ProcessPlacessAsync(csvAddresses, cancellationToken);

            var results = addressResults.Concat(placesResults).Select(x => x.Value).ToList();

            if (results.Any())
            {
                using (var writer = new StreamWriter("c:\\temp\\vic-final.tsv"))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.Delimiter = fileDelimiter;
                        csv.WriteRecords(results);
                    }
                }
            }

            Console.WriteLine("Done.");
        }

        private static async Task<IDictionary<string, Location>> ProcessAddressesAsync(IList<CsvAddress> csvAddresses,
                                                                                       CancellationToken cancellationToken)
        {
            var service = new GoogleMapsApiService(GoogleApiKey);

            // Grab addresses that have a street.
            var queries = csvAddresses.Where(x => !string.IsNullOrWhiteSpace(x.Street))
                                      .Select(x => new GeocodeQuery
                                      {
                                          Id = x.Id,
                                          Address = $"{x.Street}, {x.Suburb} {x.State}",
                                          ComponentFilters = new ComponentFilters
                                          {
                                              PostalCode = x.Postcode,
                                              CountryCodeIso = "AU"
                                          }
                                      }).ToList();

            Console.WriteLine($" -> About to geocode {queries.Count()} [of {csvAddresses.Count}] addresses ....");

            IDictionary<string, Location> results;

            if (queries.Any())
            {
                var geocodeResults = await BatchHelpers.BatchInvokeAsync(queries, service.GeocodeAsync, cancellationToken);

                // Grab all the successfull results.
                results = geocodeResults.Where(x => x.Status == "OK")
                                                     .Select(x => x.Results.FirstOrDefault())
                                                     .ToDictionary(key => key.Id, value => value);
            }
            else
            {
                results = new Dictionary<string, Location>();
            }

            Console.WriteLine($" Found: {results.Count} results.");

            return results;
        }

        private static async Task<IDictionary<string, Location>> ProcessPlacessAsync(IList<CsvAddress> csvAddresses,
                                                                                     CancellationToken cancellationToken)
        {
            var service = new GooglePlacesApiService(GoogleApiKey);

            // Try and determine each place.
            var queries = csvAddresses.Where(x => !string.IsNullOrWhiteSpace(x.Agency) &&
                                                  string.IsNullOrWhiteSpace(x.Street))
                                      .Select(x => new AutocompleteQuery
                                      {
                                          Id = x.Id,
                                          Query = $"{x.Agency} {(string.IsNullOrWhiteSpace(x.Suburb) ? string.Empty : x.Suburb)} {x.State} {x.Postcode}".Trim(),
                                          AutocompleteType = AutocompleteType.Establishment
                                      });

            Console.WriteLine($" -> About to search and geocode {queries.Count()} [of {csvAddresses.Count}] establishments ....");

            IDictionary<string, Location> results;

            if (queries.Any())
            {
                var autocompleteResults = await BatchHelpers.BatchInvokeAsync(queries, service.AutocompleteAsync, cancellationToken);

                // Grab all the successfull results.
                var detailsQueries = autocompleteResults.Where(x => x.Status == "OK")
                                                        .Select(x => x.Results.FirstOrDefault())
                                                        .Select(x => new DetailsQuery
                                                        {
                                                            Id = x.Id,
                                                            PlaceId = x.PlaceId
                                                        });

                // Foreach result, lets get the lat/long.
                var detailsResults = await BatchHelpers.BatchInvokeAsync(detailsQueries, service.DetailsAsync, cancellationToken);

                // Grab all the successfull results again.
                results = detailsResults.Where(x => x.Status == "OK")
                                        .ToDictionary(key => key.Location.Id, value => value.Location);
            }
            else
            {
                results = new Dictionary<string, Location>();
            }

            Console.WriteLine($" Found: {results.Count} results.");

            return results;
        }
    }
}
