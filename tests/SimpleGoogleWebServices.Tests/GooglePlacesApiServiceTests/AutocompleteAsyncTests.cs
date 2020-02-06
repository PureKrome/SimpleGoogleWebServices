using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using WorldDomination.SimpleGoogleWebServices;
using WorldDomination.SimpleGoogleWebServices.Autocomplete;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.Spatial.SimpleGoogleWebServices.Tests.GooglePlacesApiServiceTests
{
    public class AutocompleteAsyncTests
    {
        [Theory]
        [InlineData("Unknown Error", "InternalServerError", HttpStatusCode.InternalServerError)]
        [InlineData("Invalid Request", "INVALID_REQUEST", HttpStatusCode.OK)]
        [InlineData("Request Denied", "REQUEST_DENIED", HttpStatusCode.OK)]
        [InlineData("Zero Results", "ZERO_RESULTS", HttpStatusCode.OK)]
        public async Task GivenAnInvalidRequest_AutocompleteAsync_ReturnsAnErrorResult(string fileName,
                                                                                       string status,
                                                                                       HttpStatusCode statusCode)
        {
            // Arrange.
            var json = File.ReadAllText($"Sample Data\\Autocomplete\\{fileName}.json");
            var options = new HttpMessageOptions
            {
                
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json, statusCode)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService("aaa", httpClient);
            var autocompleteQuery = new AutocompleteQuery
            {
                Query = "whatever",
                AutocompleteType = AutocompleteType.Address
            };

            // Act.
            var result = await service.AutocompleteAsync(autocompleteQuery, CancellationToken.None);

            // Assert.
            result.Results.Count().ShouldBe(0);
            result.Status.ShouldBe(status);
        }

        [Theory]
        [InlineData("Single Prediction", 1, "Blue Hills Cres, Malanda, Queensland, Australia")]
        public async Task GivenAValidQuery_AutocompleteAsync_ReturnsAnOkResult(string fileName,
                                                                               int numberOfResults,
                                                                               string address)
        {
            // Arrange.
            var json = File.ReadAllText($"Sample Data\\Autocomplete\\{fileName}.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService("aaa", httpClient);
            var autocompleteQuery = new AutocompleteQuery
            {
                Query = "whatever",
                AutocompleteType = AutocompleteType.Address
            };

            // Act.
            var result = await service.AutocompleteAsync(autocompleteQuery, CancellationToken.None);

            // Assert.
            result.Results.Count().ShouldBe(numberOfResults);
            result.Status.ShouldBe("OK");

            var firstAddress = result.Results.First();
            firstAddress.Location.ShouldBe(address);
            firstAddress.PlaceId.ShouldNotBeNullOrWhiteSpace();
        }
    }
}