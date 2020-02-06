using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldDomination.SimpleGoogleWebServices;
using WorldDomination.SimpleGoogleWebServices.Geocode;

namespace SimpleGoogleWebServices.ConsoleApp
{
    class Program
    {
        private const string GoogleApiKey = "AIzaSyAh5QsHUGZKh1BkifV1VwBYT6s7NCfggpg";

        static async Task Main()
        {
            IList<CsvAddress> csvAddresses;
            const string fileDelimiter = "\t";

            var cancellationToken = new CancellationToken();

            // Read in a csv file.
            using (var reader = new StreamReader("c:\\temp\\adam - vic - full.tsv"))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                    csv.Configuration.Delimiter = fileDelimiter; // Tab separated.
                    csvAddresses = csv.GetRecords<CsvAddress>().ToList();
                }
            }

            var addressResults = await ProcessAddressesAsync(csvAddresses, cancellationToken);

            //var placesResults = await ProcessPlacessAsync(csvAddresses, cancellationToken);

            //if (results.Any())
            //{
            //    using (var writer = new StreamWriter("c:\\temp\\adam-vic-final.tsv"))
            //    {
            //        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //        {
            //            csv.Configuration.Delimiter = fileDelimiter;
            //            csv.WriteRecords(results);
            //        }
            //    }

            //}

            Console.WriteLine("Done.");
        }

        private static async Task<IDictionary<string, Location>> ProcessAddressesAsync(IList<CsvAddress> csvAddresses, CancellationToken cancellationToken)
        {
            // Process each item.
            var service = new GoogleMapsApiService(GoogleApiKey);

            // Grab addresses that have a street.
            var batchAddresses = csvAddresses.Where(x => string.IsNullOrWhiteSpace(x.Street))
                                             .Select(x => new GeocodeQuery
                                             {
                                                 Id = x.Id,
                                                 Address = $"{x.Street}, {x.Suburb} {x.State}",
                                                 ComponentFilters = new ComponentFilters
                                                 {
                                                     PostalCode = x.Postcode,
                                                     CountryCodeIso = "AU"
                                                 }
                                             });

            IDictionary<string, Location> results;

            if (batchAddresses.Any())
            {
                var geocodeResults = await BatchHelpers.BatchInvokeAsync(batchAddresses.Take(5), service.GeocodeAsync, cancellationToken);

                // Grab all the successfull results.
                results = geocodeResults.Where(x => x.Status == "OK")
                                                     .SelectMany(x => x.Results)
                                                     .ToDictionary(key => key.Id, value => value);
            }
            else
            {
                results = new Dictionary<string, Location>();
            }

            Console.WriteLine($" Found: {results.Count} results.");

            return results;
        }

        //private static async Task<IDictionary<string, Location>> ProcessPlacessAsync(IList<CsvAddress> csvAddresses,
        //                                                                             CancellationToken cancellationToken)
        //{
        //    var results = new Dictionary<string, Location>();
        //    var service = new GooglePlacesApiService(GoogleApiKey);

        //    // Try and determine each place.
        //    var queries = csvAddresses.Where(x => string.IsNullOrWhiteSpace(x.Street))
        //                              .Select(x => new AutocompleteQuery
        //                              {
        //                                  Id = x.Id,
        //                                  Query = $"{(string.IsNullOrWhiteSpace(x.Suburb) ? string.Empty : x.Suburb)} {x.Street}",
        //                                  AutocompleteType = AutocompleteType.Establishment
        //                              });

        //    if (queries.Any())
        //    {
        //        var autoCompleteResults = await service.AutocompleteAsync(queries, cancellationToken);

        //        if (!autoCompleteResults.Any())
        //        {
        //            // Nothing was returned :(
        //            return new Dictionary<string, Location>();
        //        }

        //        // Lets calculate the addresses for these places.

        //    }
        //}
    }
}
