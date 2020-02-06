using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using WorldDomination.SimpleGoogleWebServices;
using WorldDomination.SimpleGoogleWebServices.Geocode;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.Spatial.SimpleGoogleWebServices.Tests.GoogleMapsApiServiceTests
{
    public class GeocodeAsyncTests
    {
        [Fact]
        public async Task GivenAGeocodingErrorOccured_Geocode_ReturnsAResultWithTheErrorMessage()
        {
            // Arrange.
            var json = File.ReadAllText("Sample Data\\Geocode\\Error Result.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GoogleMapsApiService("aaa", httpClient);
            var geocodeQuery = new GeocodeQuery
            {
                Address = "whatever"
            };

            // Act.
            var result = await service.GeocodeAsync(geocodeQuery, CancellationToken.None);

            // Assert.
            result.Results.Count().ShouldBe(0);
            result.Status.ShouldBe("REQUEST_DENIED");
            result.ErrorMessage.ShouldBe(
                "The 'sensor' parameter specified in the request must be set to either 'true' or 'false'.");
        }

        [Fact]
        public async Task GivenAnInValidQuery_Geocode_ReturnsANull()
        {
            // Arrange.
            var json = File.ReadAllText("Sample Data\\Geocode\\Zero Results.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GoogleMapsApiService("aaa", httpClient);
            var geocodeQuery = new GeocodeQuery
            {
                Address = "sdfhgjshf ashdf ashdfj asd gfajskdg"
            };

            // Act.
            var result = await service.GeocodeAsync(geocodeQuery, CancellationToken.None);

            // Assert.
            result.Results.Count().ShouldBe(0);
            result.Status.ShouldBe("ZERO_RESULTS");
        }

        /// <summary>
        /// This is an example of where google cannot find the location at all. It bubbles up to the postcode.
        /// </summary>
        [Fact]
        public async Task GivenAQuery15SpinnakerRiseSanctuaryLakesVictoriaAustralia_Geocode_ReturnsSomeData()
        {
            // Arrange.
            const string streetNumber = "15";
            const string street = "Spinnaker Rise";
            //const string suburb = "Sanctuary Lakes";
            const string state = "VIC";
            const string postcode = "3030";
            const string country = "AUSTRALIA";

            var componentFilters = new ComponentFilters
            {
                PostalCode = postcode
            };
            var json = File.ReadAllText("Sample Data\\Geocode\\Result - 15 Spinnaker Rise Sanctuary Lakes Victoria.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GoogleMapsApiService("aaa", httpClient);
            var geocodeQuery = new GeocodeQuery
            {
                Address = $"{streetNumber} {street}, {state}, {country}",
                ComponentFilters = componentFilters
            };

            // Act.
            var result = await service.GeocodeAsync(geocodeQuery, CancellationToken.None);

            // Assert.
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Results.Count().ShouldBe(1);
            var data = result.Results.First();
            // NOTE: Yes! The query is for a location in Sanctuary Point (even though that exact suburb name isn't provided)
            //       and the result is for Point Cook.
            data.Address.ShouldBe("15 Spinnaker Rise, Point Cook VIC 3030, Australia");
            data.Coordinate.Latitude.ShouldBe(-37.899923m);
            data.Coordinate.Longitude.ShouldBe(144.775248m);
        }

        [Fact]
        public async Task GivenAValidQuery_Geocode_ReturnsSomeData()
        {
            // Arrange.
            const string streetNumber = "4";
            const string street = "Albert Pl";
            const string suburb = "RICHMOND";
            const string state = "VIC";
            const string postcode = "3121";
            const string country = "AUSTRALIA";

            var componentFilters = new ComponentFilters
            {
                PostalCode = postcode
            };
            var json = File.ReadAllText("Sample Data\\Geocode\\Results.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GoogleMapsApiService("aaa", httpClient);
            var geocodeQuery = new GeocodeQuery
            {
                Address = $"{streetNumber} {street}, {suburb} {state} {postcode}, {country}",
                ComponentFilters = componentFilters
            };

            // Act.
            var result = await service.GeocodeAsync(geocodeQuery, CancellationToken.None);

            // Assert.
            result.ErrorMessage.ShouldBeNullOrEmpty();
            result.Results.Count().ShouldBe(3);
            var data = result.Results.First();
            data.Address.ShouldBe("4 Albert Street, Richmond VIC 3121, Australia");
            data.Coordinate.Latitude.ShouldBe(-37.828601m);
            data.Coordinate.Longitude.ShouldBe(144.997996m);
        }
    }
}