using System;
using System.IO;
using System.Linq;
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
    public class CleanUpAddressAsyncTests
    {
        [Theory]
        [InlineData("Invalid Request", "Invalid Request", "INVALID_REQUEST", 1)] // First api call fails.
        [InlineData("Single Prediction", "Invalid Request", "INVALID_REQUEST", 2)] // Second api call fails.
        public async Task GivenAnInvalidRequest_CleanUpAddressAsync_ReturnsAnErrorResult(string autocompleteFileName,
                                                                                         string detailsFileName,
                                                                                         string status,
                                                                                         int numberOfHttpClientCalls)
        {
            // Arrange.
            const string addressQuery = "whatever";
            const string apiKey = "aaa";
            var autocompleteJson = File.ReadAllText($"Sample Data\\Autocomplete\\{autocompleteFileName}.json");
            var autocompleteResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(autocompleteJson);

            var detailsJson = File.ReadAllText($"Sample Data\\Details\\{detailsFileName}.json");
            var detailsResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(detailsJson);

            var options = new[]
            {
                new HttpMessageOptions
                {
                    RequestUri = new Uri($"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={addressQuery}&types=address&key={apiKey}"),
                    HttpResponseMessage = autocompleteResponseMessage
                },
                new HttpMessageOptions
                {
                    // NOTE: PlaceId value was manually copied/pasted from the autocomplete.json file.
                    RequestUri = new Uri($"https://maps.googleapis.com/maps/api/place/details/json?placeid=Ei9CbHVlIEhpbGxzIENyZXMsIE1hbGFuZGEsIFF1ZWVuc2xhbmQsIEF1c3RyYWxpYQ&key={apiKey}"),
                    HttpResponseMessage = detailsResponseMessage
                },
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService(apiKey, httpClient);
            var autocompleteQuery = new AutocompleteQuery
            {
                Query = addressQuery,
                AutocompleteType = AutocompleteType.Address
            };

            // Act.
            var result = await service.CleanUpAddressAsync(autocompleteQuery, CancellationToken.None);

            // Assert.
            result.Status.ShouldBe(status);
            result.Address.ShouldBeNull();
            options.Sum(x => x.NumberOfTimesCalled).ShouldBe(numberOfHttpClientCalls);
        }

        [Fact]
        public async Task GivenAnValidRequest_CleanUpAddressAsync_ReturnsAnOkResult()
        {
            // Arrange.
            const string addressQuery = "whatever";
            const string apiKey = "aaa";
            var autocompleteJson = File.ReadAllText("Sample Data\\Autocomplete\\Single Prediction.json");
            var autocompleteResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(autocompleteJson);

            var detailsJson = File.ReadAllText("Sample Data\\Details\\OK result - full address.json");
            var detailsResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(detailsJson);

            var options = new[]
            {
                new HttpMessageOptions
                {
                    RequestUri = new Uri($"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={addressQuery}&types=address&key={apiKey}"),
                    HttpResponseMessage = autocompleteResponseMessage
                },
                new HttpMessageOptions
                {
                    // NOTE: PlaceId value was manually copied/pasted from the autocomplete.json file.
                    RequestUri = new Uri($"https://maps.googleapis.com/maps/api/place/details/json?placeid=Ei9CbHVlIEhpbGxzIENyZXMsIE1hbGFuZGEsIFF1ZWVuc2xhbmQsIEF1c3RyYWxpYQ&key={apiKey}"),
                    HttpResponseMessage = detailsResponseMessage
                },
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(options));
            var service = new GooglePlacesApiService(apiKey, httpClient);
            var autocompleteQuery = new AutocompleteQuery
            {
                Query = addressQuery,
                AutocompleteType = AutocompleteType.Address
            };

            // Act.
            var result = await service.CleanUpAddressAsync(autocompleteQuery, CancellationToken.None);

            // Assert.
            result.Status.ShouldBe("OK");
            result.Address.ShouldNotBeNull();
            result.Address.StreetNumber.ShouldNotBeNullOrWhiteSpace();
            result.Address.Street.LongName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Street.ShortName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Suburb.LongName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Suburb.ShortName.ShouldNotBeNullOrWhiteSpace();
            result.Address.City.ShouldNotBeNullOrWhiteSpace();
            result.Address.State.LongName.ShouldNotBeNullOrWhiteSpace();
            result.Address.State.ShortName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Country.LongName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Country.ShortName.ShouldNotBeNullOrWhiteSpace();
            result.Address.Postcode.ShouldNotBeNullOrWhiteSpace();
            options.Sum(x => x.NumberOfTimesCalled).ShouldBe(2);
        }
    }
}