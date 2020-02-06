namespace WorldDomination.SimpleGoogleWebServices.Geocode
{
    public class GeocodeQuery
    {
        /// <summary>
        /// Optional Identifier to link the potential result back to this query.
        /// </summary>
        public string Id { get; set; }
        public string Address { get; set; }
        public ComponentFilters ComponentFilters { get; set; }
    }
}
