using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Services.Pdf.Dto
{
    public class TicketReport
    {
        private readonly FieldTicket _fieldTicket;

        private readonly string? _customerSignature;

        public TicketReport(FieldTicket fieldTicket, string? customerSignature = null)
        {
            _fieldTicket = fieldTicket;
            _customerSignature = customerSignature;
        }

        public string TicketIdentifier => _fieldTicket.TicketId;

        public string RigIdentifier => _fieldTicket.Equipment!.Name;

        public string Customer => _fieldTicket.Customer!.Name;

        public string Address => _fieldTicket.Customer!.Address;

        public string City => _fieldTicket.Customer!.City;

        public string State => _fieldTicket.Customer!.State;

        public string Zip => _fieldTicket.Customer!.Zip;

        public string Lease => _fieldTicket.Location?.Lease ?? string.Empty;

        public string Well => _fieldTicket.Location?.Well ?? string.Empty;

        public string Field => _fieldTicket.Location?.Field ?? string.Empty;

        public string County => _fieldTicket.Location?.County ?? string.Empty;

        public string ExecutionDate => _fieldTicket.ExecutionDate.ToString("yyyy-MM-dd");

        public string StartTime => _fieldTicket.StartTime.HasValue ? _fieldTicket.StartTime.Value.ToString("HH:mm") : string.Empty;

        public string EndTime => _fieldTicket.EndTime.HasValue ? _fieldTicket.EndTime.Value.ToString("HH:mm") : string.Empty;

        public string Description => _fieldTicket.Description;

        public double Subtotal => _fieldTicket.TicketSpecifications.Select(x => new { Total = x.Quantity * x.Rate }).Sum(x => x.Total);

        public string TaxRate => $"{_fieldTicket.TaxRate:#.##}%";

        public double Total => _fieldTicket.TaxRate > 0 ? Subtotal * (1 + _fieldTicket.TaxRate / 100) : Subtotal;

        public string CustomerPrintedName => _fieldTicket.CustomerPrintedName;

        public string CustomerSignature => _customerSignature ?? string.Empty;

        public string CustomerSignedOn => _fieldTicket.CustomerSignedOn?.ToString("yyyy-MM-dd") ?? string.Empty;

        public string EmployeePrintedName => _fieldTicket.EmployeeSignature;

        public string EmployeeSignature => _fieldTicket.EmployeePrintedName;

        public string EmployeeSignedOn => _fieldTicket.SignedOn?.ToString("yyyy-MM-dd") ?? string.Empty;

        public List<TicketPayrollData> PayrollData => _fieldTicket.PayrollData.Select(x => new TicketPayrollData(_fieldTicket, x)).ToList();

        public List<(TicketReportCharge LeftSide, TicketReportCharge RightSide)> Charges()
        {
            var result = new List<(TicketReportCharge, TicketReportCharge)>();
            var chargesCount = _fieldTicket.TicketSpecifications.Count;
            var half = (int)Math.Ceiling((decimal)chargesCount / 2);
            var leftSide = _fieldTicket.TicketSpecifications.Take(half).ToList();
            var rightSide = _fieldTicket.TicketSpecifications.Skip(half).ToList();
            for (int i = 0; i < half; i++)
            {
                var leftSideCharge = leftSide[i];
                var rightSideCharge = rightSide.Count < i ? rightSide[i] : null;
                result.Add((new TicketReportCharge(leftSideCharge), new TicketReportCharge(rightSideCharge)));
            }

            return result;
        }

        public string GetWellType(WellType wellType)
        {
            if (_fieldTicket.Location == null)
            {
                return string.Empty;
            }

            return wellType == _fieldTicket.Location.WellType ? "X" : string.Empty;
        }

        public string GetServiceTypeSelection(ServiceType? serviceType = null)
        {
            if (serviceType.HasValue)
            {
                return _fieldTicket.ServiceType == serviceType.Value ? "X" : "";
            }

            return _fieldTicket.ServiceType.GetServiceType();
        }
    }

    public class TicketReportCharge
    {
        public string Item { get; set; } = string.Empty;

        public string Quantity { get; set; } = string.Empty;

        public string UoM { get; set; } = string.Empty;

        public string Rate { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;

        public TicketReportCharge(TicketSpecification? ticketSpecification)
        {
            if (ticketSpecification == null)
            {
                return;
            }

            Item = ticketSpecification.Charge;
            UoM = ticketSpecification.UoM.ToString();
            Rate = ticketSpecification.Rate.ToString("#.##");
            Quantity = ticketSpecification.Quantity.ToString();
            Amount = (ticketSpecification.Quantity * ticketSpecification.Rate).ToString("#.##");
        }
    }

    public class TicketPayrollData
    {
        public string Labor { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public double RigHours { get; set; }

        public double Travel { get; set; }

        public double Yard { get; set; }

        public double Roustabout { get; set; }

        public double TotalTime => RigHours + Travel + Yard + Roustabout;

        public TicketPayrollData(FieldTicket ticket, PayrollData payrollData)
        {
            Labor = payrollData.Employee != null ? payrollData.Employee.JobTitle.GetJobTitle() : "Other";
            Name = payrollData.Employee != null ? payrollData.Employee.DisplayName : payrollData.Name;
            EmployeeNumber = payrollData.Employee != null ? payrollData.Employee.EmployeeNumber : "0000";
            RigHours = ticket.TicketSpecifications.SingleOrDefault(x => x.Charge == ChargeNames.Rig)?.Quantity ?? 0;
            Travel = ticket.TicketSpecifications.SingleOrDefault(x => x.Charge == ChargeNames.TravelTime)?.Quantity ?? 0;
            Yard = ticket.TicketSpecifications.SingleOrDefault(x => x.Charge == ChargeNames.Rig)?.Quantity ?? 0;
            Roustabout = ticket.TicketSpecifications.SingleOrDefault(x => x.Charge == ChargeNames.Rig)?.Quantity ?? 0;
        }
    }
}
