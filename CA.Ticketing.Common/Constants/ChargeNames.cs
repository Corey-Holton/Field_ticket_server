namespace CA.Ticketing.Common.Constants
{
    public class ChargeNames
    {
        public const string Permit = nameof(Permit);

        public const string Rig = nameof(Rig);

        public const string PumpOrTank = "Pump/Tank";

        public const string BOP = nameof(BOP);

        public const string PowerSwivelOrSub = "Power Swivel/SUB";

        public const string TTWValve = "TTW Valve";

        public const string HydraulicRodTongs = "Hydraulic Rod Tongs";

        public const string ToolPusher = "Tool Pusher";

        public const string ExtraHand = "Extra Hand";

        public const string Fuel = "Fuel";

        public const string OvershotStanding = "Overshot/Standing";

        public const string Valve = nameof(Valve);

        public const string PipeDope = "Pipe Dope";

        public const string RodStripperRubber = "Rod Stripper Rubber";

        public const string PerDiem = "Per Diem";

        public const string TbgStripperRubber = "Tbg Stripper Rubber";

        public const string TubingWiper = "Tubing Wiper";

        public const string SwabCups = "Swab Cups";

        public const string OilSaverRubber = "Oil Saver Rubber";

        public const string Paint = nameof(Paint);

        public const string TravelTime = "Travel Time";

        public const string CrewTruck = "Crew Truck";

        public const string PipeTrailer = "Pipe Trailer";

        public const string PipeRacks = "Pipe Racks/Catwalk";

        public const string ThirdParty = "Third Party Charges";

        public const string Trucking = nameof(Trucking);

        public const string Other = nameof(Other);

        public const string Labor = nameof(Labor);
    }

    public class ChargesInfo
    {
        private static readonly List<string> _readonlyCharges = new() 
        { 
            ChargeNames.Rig,
            ChargeNames.Fuel,
            ChargeNames.Labor
        };
        
        public static List<string> ReadonlyCharges => _readonlyCharges;
    }
}
