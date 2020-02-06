namespace WorldDomination.SimpleGoogleWebServices.Autocomplete
{
    public class AutocompleteQuery
    {
        /// <summary>
        /// Optional Identity to correlate any results against.
        /// </summary>
        public string Id { get; set; }
        public string Query { get; set; }
        public AutocompleteType AutocompleteType { get; set; }
    }
}
