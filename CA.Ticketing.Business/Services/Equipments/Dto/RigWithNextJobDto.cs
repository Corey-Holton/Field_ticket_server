namespace CA.Ticketing.Business.Services.Equipments.Dto
{
    public class RigWithNextJobDto
    {
        public EquipmentDto Rig { get; set; }
        public int DaysSinceLastJob { get; set; }
        public int DaysUntilNextJob { get; set; }
    }
}
