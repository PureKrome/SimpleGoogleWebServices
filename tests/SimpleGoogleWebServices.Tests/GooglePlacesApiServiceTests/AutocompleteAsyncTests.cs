using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using WorldDomination.SimpleGoogleWebServices;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.Spatial.SimpleGoogleWebServices.Tests.GooglePlacesApiServiceTests
{
    public class AutocompleteAsyncTests
    {
        [Theory]
        [InlineData("Invalid Request", "INVALID_REQUEST")]
        [InlineData("Request Denied", "REQUEST_DENIED")]
        [InlineData("Zero Results", "ZERO_RESULTS")]
        public async Task GivenAnInvalidRequest_AutocompleteAsync_ReturnsAnErrorResult(string fileName,
                                                                                       string status)
        {
            // Arrange.
            var json = File.ReadAllText($"Sample Data\\Autocomplete\\{fileName}.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService("aaa", httpClient);

            // Act.
            var result = await service.AutocompleteAsync("whatever");

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

            // Act.
            var result = await service.AutocompleteAsync("whatever");

            // Assert.
            result.Results.Count().ShouldBe(numberOfResults);
            result.Status.ShouldBe("OK");

            var firstAddress = result.Results.First();
            firstAddress.Location.ShouldBe(address);
            firstAddress.PlaceId.ShouldNotBeNullOrWhiteSpace();
        }
    }
}