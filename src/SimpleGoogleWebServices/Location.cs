namespace WorldDomination.SimpleGoogleWebServices
{
    public class Location
    {
        /// <summary>
        /// Optional Identifier.
        /// </summary>
        public string Id { get; set; }
        public string Address { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}