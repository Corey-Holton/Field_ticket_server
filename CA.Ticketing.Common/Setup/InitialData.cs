namespace CA.Ticketing.Common.Setup
{
    public class InitialData
    {
        public InitialDataIdentifier[] Tickets { get; set; }

        public InitialDataIdentifier Invoices { get; set; }
    }

    public class InitialDataIdentifier
    {
        public string Identifier { get; set; }

        public int StartId { get; set; }
    }
}
