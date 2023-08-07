namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class EquipmentDetailsDto : EquipmentDto
    {
        public int? Year { get; set; }

        public string VinNumber { get; set; }

        public string PermitNumber { get; set; }

        public double FuelConsumption { get; set; }
    }
}
