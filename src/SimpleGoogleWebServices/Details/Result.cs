using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WorldDomination.SimpleGoogleWebServices.Details
{
    internal class Result
    {
        [JsonProperty("address_components")]
        public List<AddressComponent> AddressComponents { get; set; } = new List<AddressComponent>();

        [JsonProperty("adr_address")]
        public string AdrAddress { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        public Geometry Geometry { get; set; }
        public string Icon { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        public string Reference { get; set; }
        public string Scope { get; set; }
        public List<string> Types { get; set; }
        public string Url { get; set; }

        [JsonProperty("utc_offset")]
        public int UtcOffset { get; set; }

        public string Vicinity { get; set; }

        public Address ToAddress
        {
            get
            {
                if (AddressComponents?.Any() != true)
                {
                    return null;
                }

                var streetNumber = AddressComponents.FirstOrDefaultWithType("street_number");
                var street = AddressComponents.FirstOrDefaultWithAnyType(new[]
                {
                    "route",
                    "street_address"
                });
                var suburb = AddressComponents.FirstOrDefaultWithAnyType(new[]
                {
                    "locality",
                    "neighborhood"
                });
                var city = AddressComponents.FirstOrDefaultWithType("administrative_area_level_2");
                var state = AddressComponents.FirstOrDefaultWithType("administrative_area_level_1");
                var country = AddressComponents.FirstOrDefaultWithType("country");
                var postcode = AddressComponents.FirstOrDefaultWithType("postal_code");

                return new Address
                {
                    StreetNumber = streetNumber?.LongName,
                    Street = street != null ? ToNames(street) : null,
                    Suburb = suburb != null ? ToNames(suburb) : null,
                    City = city?.LongName,
                    State = state != null ? ToNames(state) : null,
                    Country = country != null ? ToNames(country) : null,
                    Postcode = postcode?.LongName
                };
            }
        }

        public Names ToNames(AddressComponent addressComponent)
        {
            if (addressComponent == null)
            {
                throw new System.ArgumentNullException(nameof(addressComponent));
            }

            return new Names
            {
                LongName = addressComponent.LongName,
                ShortName = addressComponent.ShortName
            };
        }
    }
}