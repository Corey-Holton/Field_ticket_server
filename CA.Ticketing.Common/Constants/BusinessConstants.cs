﻿namespace CA.Ticketing.Common.Constants
{
    public class BusinessConstants
    {
        public class LocalServer
        {
            public const string ApiBaseUrl = "http://localhost:7057";

            public const string WebBaseUrl = "http://localhost:7058";
        }

        public class JobTitles
        {
            public const string ToolPusher = "Tool Pusher";
            public const string CrewChief = "Crew Chief";
            public const string DerrickMan = "Derrick Man";
            public const string FloorHand = "Floor Hand";
            public const string Other = nameof(Other);
        }

        public class WellRecordTypes
        {
            public const string NumRodSubs = "Number of Rod Subs";
            public const string NumRods = "Number of Rods";
            public const string PumpNum = "Pump #";
            public const string GasAnchor = "Gas Anchor/Strainer";
            public const string NumTubing = "Number of Tubing Subs";
            public const string NumJoints = "Number of Joints (Tubing or Pipe)";
        }

        public class LocationTypes
        {
            public const string HQ = "HQ";
            public const string Field = "Field";
        }

        public class ServiceTypes
        {
            public const string RodsAndTubing = "Rods And Tubing";
            public const string Workover = nameof(Workover);
            public const string PAndA = "P&A";
            public const string Completion = nameof(Completion);
            public const string StandBy = "Stand By";
            public const string Roustabout = nameof(Roustabout);
            public const string Yard = nameof(Yard);
        }

        public const double InvoiceLateFee = 1.5;

        public class BackgroundJobNames
        {
            public const string InvoiceLateFees = nameof(InvoiceLateFee);
        }
    }
}
