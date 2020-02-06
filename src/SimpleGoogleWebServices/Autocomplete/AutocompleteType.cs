namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    // NOTE: technically, this should be a 'FLAGS' but we're restricting the usage of this
    //       to keep things simple.
    public enum AutocompleteType
    {
        Address,
        Establishment
    }

    public static class AutocompleteTypeExtensions
    {
        public static string ToQueryStringParameter(this AutocompleteType autocompleteType)
        {
            return autocompleteType switch
            {
                AutocompleteType.Address => "address",
                AutocompleteType.Establishment => "establishment",
                _ => string.Empty,
            };
        }
    }
}
