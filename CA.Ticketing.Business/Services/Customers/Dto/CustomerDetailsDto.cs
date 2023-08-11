namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDetailsDto : CustomerDto
    {
        public string Address { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public List<CustomerLocationDto> Locations { get; set; }

        public List<CustomerContactDto> Contacts { get; set; }
    }
}
