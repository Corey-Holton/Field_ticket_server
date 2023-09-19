namespace CA.Ticketing.Persistance.Models.Abstracts
{
    public class IdentityModelWithAddress : IdentityModel
    {
        public string Address { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }
    }
}
