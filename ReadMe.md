# Spatial Utilities for .NET applications

[![Build status](https://ci.appveyor.com/api/projects/status/ooqkpqtsyy2vuor1?svg=true)](https://ci.appveyor.com/project/PureKrome/simplegooglewebservices) [![NuGet Badge](https://buildstats.info/nuget/WorldDomination.SimpleGoogleWebServices)](https://www.nuget.org/packages/WorldDomination.SimpleGoogleWebServices/) [![MyGet Badge](https://buildstats.info/myget/pk-development/WorldDomination.SimpleGoogleWebServices)](https://www.myget.org/feed/pk-development/package/nuget/WorldDomination.SimpleGoogleWebServices) [![license](https://img.shields.io/github/license/mashape/apistatus.svg)]()

This library contains various some simple .NET wrapper code over some google webservices.

Currently, it's wrapping code over some maps/places webservices.

NOTE: _This simple library is **not** intended to replace official Google .NET SDK's. It's only to simplfiy calling some of their simple endpoints._

- `GeocodeAsync` : convert an address into a Latitude/Longitude.
- `AutocompleteAsync` : convert a query into a list of possible addresses.
- `DetailsAsync` : convert a Google PlaceId into a nicely Address object.
- `CleanUpAddressAsync` : convert an address's abbreviations all into long format. e.g. St. (for street) -> Street. NOTE: This calls `Autocomplete` and then `Details`.

# TODO - clean this up.

### GeocodeAsync
Given an query/address, this get's the Latitude and Longitude of the location.

    // Arrange.
	var service = new GoogleMapsApiService(yourGoogleMapsApiKey, null || mockHttpClient);
    
    // Act.
    var result = await service.GeocodeAsync("Bondi Beach, Sydney, Australia");
    
    // Now you can access:
    // result.Address
    // result.Coordinate.Latitude
	// result.Coordinate.Longitude

Remarks: Learn what [geocoding is on Wikipedia(http://en.wikipedia.org/wiki/Geocoding).

### AutocompleteAsync
Given a query lets see what possible address locations might be available.
NOTE: `PlaceId` is a specific, unique Google Id to identify the result location.

    // Arrange.
	var service = new GooglePlacesApiService(yourGoogleMapsApiKey, null || mockHttpClient);
    
    // Act.
    var result = await service.AutocompleteAsync("Bondi Beach, Sydney, Australia");
    
    // Now you can access:
    // result.Address.PlaceId
    // result.Address.Location


### DetailsAsync
Given a (Google) PlaceId, return the sepcific, verbose Address information for that location.

    // Arrange.
	var service = new GooglePlacesApiService(yourGoogleMapsApiKey, null || mockHttpClient);
    
    // Act.
    var result = await service.DetailsAsync("1234ABCD...");
    
    // Now you can access:
    // result.Address.StreetNumber
    // result.Address.Street
	// result.Address.Suburb
	// result.Address.City
	// result.Address.State
	// result.Address.Country
	// result.Address.Postcode


### CleanUpAddressAsync
Given an exact address (because we don't know the `PlaceId`) get the details for the (expected) found location.<br/>
*Note:* This first calls `AutocompleteAsync` and if we have 1 result exactly, then calls `DetailsASync` and extracts the long form address components.


    // Arrange.
	var service = new GooglePlacesApiService(yourGoogleMapsApiKey, null || mockHttpClient);
    
    // Act.
    var result = await service.CleanUpAsync("1 Bondi Beach, Sydney, Australia");
    
    // Now you can access:
    // result.Address.StreetNumber
    // result.Address.Street
	// result.Address.Suburb
	// result.Address.City
	// result.Address.State
	// result.Address.Country
	// result.Address.Postcode


License: this code is licensed under MIT.
-- end of file --