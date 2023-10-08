using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Services.Pdf.Dto
{
    public class InvoiceReport
    {
        private readonly Invoice _invoice;

        public InvoiceReport(Invoice invoice)
        {
            _invoice = invoice;
            GenerateTickets();

            if (!invoice.InvoiceLateFees.Any())
            {
                LateFeesTotal = 0;
            }

            TotalWithFees = Total;

            foreach (var _ in invoice.InvoiceLateFees)
            {
                TotalWithFees += TotalWithFees * BusinessConstants.InvoiceLateFee / 100;
            }

            LateFeesTotal = TotalWithFees - Total;
        }

        public List<InvoiceFieldTicket> Tickets { get; set; } = new List<InvoiceFieldTicket>();

        public string CustomerName => _invoice.Customer.Name;

        public string CustomerAddress => _invoice.Customer.Address;

        public string CustomerCity => $"{_invoice.Customer.City}, {_invoice.Customer.State} {_invoice.Customer.Zip}";

        public string InvoiceNumber => _invoice.InvoiceId;

        public string Terms => $"Net {_invoice.Customer.NetTerm}";

        public string InvoiceDate => _invoice.InvoiceDate.ToUSDateTime();

        public string InvoiceDueDate => _invoice.DueDate.ToUSDateTime();

        public double Subtotal => Tickets.Sum(x => x.SubTotal);

        public double SalesTax => Tickets.Sum(x => x.TaxTotal);

        public double Total => Tickets.Sum(x => x.Total);

        public double LateFeesTotal { get; set; } = 0;

        public double TotalWithFees { get; set; }

        private void GenerateTickets()
        {
            Tickets.AddRange(_invoice.Tickets.Select(x => new InvoiceFieldTicket(x)));
        }
    }
}

public class InvoiceFieldTicket
{
    public string ServiceType { get; set; }

    public string ServiceDate { get; set; }

    public string Description { get; set; }

    public double SubTotal { get; set; }

    public double TaxTotal { get; set; }

    public double Total { get; set; }

    public InvoiceFieldTicket(FieldTicket ticket)
    {
        ServiceType = ticket.ServiceType.GetServiceType();
        ServiceDate = ticket.ExecutionDate.ToUSDateTime();
        Description = $"{ServiceType} on {ticket.Location?.DisplayName}, Field Ticket # {ticket.TicketId}";
        SubTotal = ticket.Total;
        TaxTotal = SubTotal * ticket.TaxRate / 100;
        Total = SubTotal + TaxTotal;
    }
}
