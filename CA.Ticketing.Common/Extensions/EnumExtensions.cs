using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using static CA.Ticketing.Common.Constants.BusinessConstants;

namespace CA.Ticketing.Common.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> GetJobsSelection()
        {
            return Enum.GetValues(typeof(JobTitle))
                .Cast<JobTitle>()
                .Select(x => new SelectListItem { Value = x.ToString(), Text = x.GetJobTitle()});
        }

        public static string GetJobTitle(this JobTitle jobTitle)
        {
            return jobTitle switch
            {
                JobTitle.ToolPusher => JobTitles.ToolPusher,
                JobTitle.CrewChief => JobTitles.CrewChief,
                JobTitle.DerrickMan => JobTitles.DerrickMan,
                JobTitle.FloorHand => JobTitles.FloorHand,
                JobTitle.Other => JobTitles.Other,
                _ => throw new ArgumentException(null, nameof(jobTitle))
            };
        }

        public static int GetWellRecordAmount(this WellRecordType wellRecordType)
        {
            return wellRecordType switch
            {
                WellRecordType.NumRodSubs => 4,
                WellRecordType.NumRods => 4,
                WellRecordType.FirstPump => 1,
                WellRecordType.SecondPump => 1,
                WellRecordType.GasAnchor => 1,
                WellRecordType.NumTubing => 4,
                WellRecordType.NumJoints => 4,
                _ => throw new ArgumentException(null, nameof(wellRecordType))
            };
        }

        public static string GetWellRecordType(this WellRecordType wellRecordType, string? pumpNumber = null)
        {
            return wellRecordType switch
            {
                WellRecordType.NumRodSubs => WellRecordTypes.NumRodSubs,
                WellRecordType.NumRods => WellRecordTypes.NumRods,
                WellRecordType.FirstPump => WellRecordTypes.PumpNum + " " + pumpNumber,
                WellRecordType.SecondPump => WellRecordTypes.PumpNum + " " + pumpNumber,
                WellRecordType.GasAnchor => WellRecordTypes.GasAnchor,
                WellRecordType.NumTubing => WellRecordTypes.NumTubing,
                WellRecordType.NumJoints => WellRecordTypes.NumJoints,
                _ => throw new ArgumentException(null, nameof(wellRecordType))
            };
        }

        public static string GetPumpName(int pumpNumber)
        {
            return WellRecordTypes.PumpNum + pumpNumber;
        }

        public static string GetLocationType(this LocationType locationType)
        {
            return locationType switch
            {
                LocationType.Field => LocationTypes.Field,
                LocationType.HQ => LocationTypes.HQ,
                _ => throw new ArgumentNullException(null, nameof(locationType))
            };
        }
      
        public static string GetServiceType(this ServiceType serviceType) 
        {
            return serviceType switch
            {
                ServiceType.RodsAndTubing => ServiceTypes.RodsAndTubing,
                ServiceType.PAndA => ServiceTypes.PAndA,
                ServiceType.Completion => ServiceTypes.Completion,
                ServiceType.Yard => ServiceTypes.Yard,
                ServiceType.Workover => ServiceTypes.Workover,
                ServiceType.StandBy => ServiceTypes.StandBy,
                ServiceType.Roustabout => ServiceTypes.Roustabout,
                _ => throw new ArgumentNullException(null, nameof(serviceType))
            };
        }

        public static string GetRoleName(this ApplicationRole role)
        {
            return role switch
            {
                ApplicationRole.Admin => RoleNames.Admin,
                ApplicationRole.Manager => RoleNames.Manager,
                ApplicationRole.ToolPusher => RoleNames.ToolPusher,
                ApplicationRole.Customer => RoleNames.Customer,
                _ => throw new ArgumentNullException(null, nameof(role))
            };
        }
    }
}
