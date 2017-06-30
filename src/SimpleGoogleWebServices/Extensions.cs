using System.Collections.Generic;
using System.Linq;

namespace WorldDomination.SimpleGoogleWebServices
{
    internal static class Extensions
    {
        public static AddressComponent FirstOrDefaultWithType(this List<AddressComponent> value,
                                                              string type)
        {
            return value.FirstOrDefaultWithAnyType(new[]
            {
                type
            });
        }

        public static AddressComponent FirstOrDefaultWithAnyType(this List<AddressComponent> value,
                                                                 IEnumerable<string> types)
        {
            return value.FirstOrDefault(t => t.Types != null &&
                                             t.Types.Intersect(types).Any());
        }
    }
}