using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using WorldDomination.SimpleGoogleWebServices;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.Spatial.SimpleGoogleWebServices.Tests.GooglePlacesApiServiceTests
{
    public class DetailsAsyncTests
    {
        [Theory]
        [InlineData("OK result - full address", "1")]
        [InlineData("OK result - no street number", null)]
        public async Task GivenAValidPlaceId_DetailsAsync_ReturnsAnOkResult(string filename,
                                                                            string streetNumber)
        {
            // Arrange.
            var json = File.ReadAllText($"Sample Data\\Details\\{filename}.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService("aaa", httpClient);

            // Act.
            var result = await service.DetailsAsync("whatever");

            // Assert.
            result.Status.ShouldBe("OK");
            result.Address.ShouldNotBeNull();
            result.Address.StreetNumber.ShouldBe(streetNumber);
        }

        [Theory]
        [InlineData("Unknown Error", "InternalServerError", HttpStatusCode.InternalServerError)]
        [InlineData("Invalid Request", "INVALID_REQUEST", HttpStatusCode.OK)]
        [InlineData("Request Denied", "REQUEST_DENIED", HttpStatusCode.OK)]
        public async Task GivenAnInvalidRequest_DetailsAsync_ReturnsAnErrorResult(string filename,
                                                                                  string status,
                                                                                  HttpStatusCode statusCode)
        {
            // Arrange.
            var json = File.ReadAllText($"Sample Data\\Details\\{filename}.json");
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(json, statusCode)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService("aaa", httpClient);

            // Act.
            var result = await service.DetailsAsync("whatever");

            // Assert.
            result.Status.ShouldBe(status);
            result.Address.ShouldBeNull();
        }
    }
}