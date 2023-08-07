namespace CA.Ticketing.Business.Services.Settings.Dto
{
    public class SettingDto
    {
        public double TaxRate { get; set; }

        public double OvertimePercentageIncrease { get; set; }

        public int MileageCost { get; set; }

        public double FuelCalculationMultiplier { get; set; }
    }
}
