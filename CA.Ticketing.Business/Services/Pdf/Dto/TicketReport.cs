using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;
using System.Globalization;

namespace CA.Ticketing.Business.Services.Pdf.Dto
{
    public class TicketReport
    {
        private readonly FieldTicket _fieldTicket;

        private readonly string? _customerSignature;

        private readonly string? _employeeNumber;

        public TicketReport(FieldTicket fieldTicket, string? employeeNumber = null, string? customerSignature = null)
        {
            _fieldTicket = fieldTicket;
            _customerSignature = customerSignature;
            _employeeNumber = employeeNumber;
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

        public string ExecutionDate => _fieldTicket.ExecutionDate.ToString("dddd, MMMM dd yyyy", CultureInfo.GetCultureInfo("en-US"));

        public string StartTime => _fieldTicket.StartTime.HasValue ? _fieldTicket.StartTime.Value.ToString("hh:mm tt") : string.Empty;

        public string EndTime => _fieldTicket.EndTime.HasValue ? _fieldTicket.EndTime.Value.ToString("hh:mm tt") : string.Empty;

        public string Description => _fieldTicket.Description;

        public double Subtotal => _fieldTicket.Total;

        public string TaxRate => $"{_fieldTicket.TaxRate:#.##}%";

        public double Total => _fieldTicket.TaxRate > 0 ? Subtotal * (1 + _fieldTicket.TaxRate / 100) : Subtotal;

        public string CustomerPrintedName => _fieldTicket.CustomerPrintedName;

        public string CustomerSignature => _customerSignature ?? string.Empty;

        public string CustomerSignedOn => _fieldTicket.CustomerSignedOn?.ToString("MM-dd-yyyy") ?? string.Empty;

        public string EmployeePrintedName => _fieldTicket.EmployeePrintedName;

        public string EmployeeSignature => _fieldTicket.EmployeeSignature;

        public string EmployeeSignedOn => _fieldTicket.SignedOn?.ToString("MM-dd-yyyy") ?? string.Empty;

        public string EmployeeNumber => _employeeNumber ?? string.Empty;

        public string TicketType => _fieldTicket.TicketType?.Name ?? "Base";
       
        public string ClassName { get; set; }

        public List<TicketPayrollData> PayrollData => _fieldTicket.PayrollData
            .Select(x => new TicketPayrollData(x))
            .OrderBy(x => x.JobTitle)
            .ToList();

        public List<(TicketReportCharge LeftSide, TicketReportCharge RightSide)> Charges()
        {
            var result = new List<(TicketReportCharge, TicketReportCharge)>();
            _fieldTicket.TicketSpecifications = _fieldTicket.TicketSpecifications.OrderBy(x => x.CreatedDate).ToList();
            var chargesCount = _fieldTicket.TicketSpecifications.Count;
            var half = (int)Math.Ceiling((decimal)chargesCount / 2);
            var leftSide = _fieldTicket.TicketSpecifications.Take(half).ToList();
            var rightSide = _fieldTicket.TicketSpecifications.Skip(half).ToList();
            for (int i = 0; i < half; i++)
            {
                var leftSideCharge = leftSide[i];
                var rightSideCharge = i < rightSide.Count ? rightSide[i] : null;
                result.Add((new TicketReportCharge(leftSideCharge), new TicketReportCharge(rightSideCharge)));
            }

            return result;
        }

        public List<TicketReportWellCharge> ChargesWellType()
        {
            _fieldTicket.TicketSpecifications = _fieldTicket.TicketSpecifications.OrderBy(x => x.CreatedDate).ToList();
            var result = _fieldTicket.TicketSpecifications.Where(x => !x.SpecialCharge && x.Quantity != 0).Select(x => new TicketReportWellCharge(x)).ToList();
            return result;
        }

        public List<TicketReportWellCharge> SpecialWellCharges()
        {
            _fieldTicket.TicketSpecifications = _fieldTicket.TicketSpecifications.Where(x => x.SpecialCharge).OrderBy(x => x.CreatedDate).ToList();
            var result = _fieldTicket.TicketSpecifications.Select(x => new TicketReportWellCharge(x)).ToList();
            return result;
        }
        public List<(WellRecordType name, int amount)> WellTypes()
        {
            var result = new List<(WellRecordType name, int amount)>();
            
            foreach(var i in (WellRecordType[])Enum.GetValues(typeof(WellRecordType))) {
                result.Add(new (i, i.GetWellRecordAmount()));
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
            var serviceTypes = _fieldTicket.ServiceTypes.ToList();
            if (serviceType.HasValue)
            {
                return serviceTypes.Contains(serviceType.Value) ? "X" : "";
            }

            ServiceType? otherServiceType = _fieldTicket.ServiceTypes.Any(x => x > ServiceType.Completion) ? _fieldTicket.ServiceTypes.First(x => x > ServiceType.Completion) : null;
            return otherServiceType?.GetServiceType() ?? string.Empty;
        }
        public List<TicketRecordWell> WellRecords()
        {

            _fieldTicket.WellRecord = _fieldTicket.WellRecord.OrderBy(x => x.WellRecordType).ToList();
            var result = _fieldTicket.WellRecord.Select(x => new TicketRecordWell(x)).ToList();
            return result;
        }

        public List<TicketRecordSwab> SwabRecords()
        {
            var result = _fieldTicket.SwabCups.Select(x => new TicketRecordSwab(x)).ToList();
            return result;
        }

        public string WellOtherDetail()
        {
            var otherText = _fieldTicket.OtherText;
            return otherText;
        }
    }
}
public class TicketRecordWell
{
    public string Name { get; set; } = string.Empty;

    public WellRecordType Type { get; set; }


    public string Pulled { get; set; } = string.Empty;

    public string Ran { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string? pumpNumber { get; set; }

    public TicketRecordWell(WellRecord? wellRecord)
    {
        if (wellRecord == null)
        {
            return;
        }
        Type = wellRecord.WellRecordType;
        Name = wellRecord.WellRecordType.GetWellRecordType(wellRecord.Pump_Number);
        Pulled = wellRecord.Pulled.ToString();
        Ran = wellRecord.Ran.ToString();
        Size = wellRecord.Size;
        pumpNumber = wellRecord.Pump_Number;

    }
}


public class TicketRecordSwab
{
    public string Number { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Amount { get; set; } = string.Empty;

    public TicketRecordSwab(SwabCups? swabCups)
    {
        if (swabCups == null)
        {
            return;
        }

        Number = swabCups.Number.ToString();
        Size = swabCups.Size.ToString();
        Description = swabCups.Description;
        Amount = swabCups.Amount.ToString();
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
            Rate = ticketSpecification.Rate.ToString("N2");
            Quantity = ticketSpecification.Quantity.ToString();
            Amount = (ticketSpecification.Quantity * ticketSpecification.Rate).ToString("N2");
        }
    }

public class TicketReportWellCharge : TicketReportCharge
{
    public string Description { get; set; } = string.Empty;

    public TicketReportWellCharge(TicketSpecification? ticketSpecification) : base(ticketSpecification)
    {
        if (ticketSpecification == null)
        {
            return;
        }

        Item = ticketSpecification.Charge;
        UoM = ticketSpecification.UoM.ToString();
        Rate = ticketSpecification.Rate.ToString("N2");
        Quantity = ticketSpecification.Quantity.ToString();
        Amount = (ticketSpecification.Quantity * ticketSpecification.Rate).ToString("N2");
        Description = ticketSpecification.Charge + " " + ticketSpecification.UoM.ToString() + " " + ticketSpecification.Rate.ToString("N2") + ticketSpecification.Quantity.ToString();
    }
}

public class TicketPayrollData
    {
        public JobTitle JobTitle { get; set; }

        public string Labor { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public double RigHours { get; set; }

        public double Travel { get; set; }

        public double Yard { get; set; }

        public double Roustabout { get; set; }

        public double TotalTime => RigHours + Travel + Yard + Roustabout;

        public TicketPayrollData(PayrollData payrollData)
        {
            JobTitle = payrollData.Employee != null ? payrollData.Employee.JobTitle : JobTitle.Other;
            Labor = payrollData.Employee != null ? payrollData.Employee.JobTitle.GetJobTitle() : "Other";
            Name = payrollData.Employee != null ? payrollData.Employee.DisplayName : payrollData.Name;
            EmployeeNumber = payrollData.Employee != null ? payrollData.Employee.EmployeeNumber : "0000";
            RigHours = payrollData.RigHours;
            Travel = payrollData.TravelHours;
            Yard = payrollData.YardHours;
            Roustabout = payrollData.RoustaboutHours;
        }
    }
