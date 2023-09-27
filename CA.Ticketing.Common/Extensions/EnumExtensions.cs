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
