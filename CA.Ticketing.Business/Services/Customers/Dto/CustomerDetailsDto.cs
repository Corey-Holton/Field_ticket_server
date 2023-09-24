namespace CA.Ticketing.Business.Services.Customers.Dto
{
    public class CustomerDetailsDto : CustomerDto
    {
        public string Address { get; set; } = string.Empty;

        public string Zip { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string SendInvoiceTo { get; set; } = string.Empty;

        public List<CustomerLocationDto> Locations { get; set; } = new List<CustomerLocationDto>();

        public List<CustomerContactDto> Contacts { get; set; } = new List<CustomerContactDto>();
    }
}
