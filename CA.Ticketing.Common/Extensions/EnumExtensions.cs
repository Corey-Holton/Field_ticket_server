using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                JobTitle.ToolPusher => Business.JobTitles.ToolPusher,
                JobTitle.CrewChief => Business.JobTitles.CrewChief,
                JobTitle.DerrickMan => Business.JobTitles.DerrickMan,
                JobTitle.FloorHand => Business.JobTitles.FloorHand,
                _ => throw new ArgumentException(null, nameof(jobTitle))
            };
        }

        public static string GetLocationType(this LocationType locationType)
        {
            return locationType switch
            {
                LocationType.Field => Business.LocationTypes.Field,
                LocationType.HQ => Business.LocationTypes.HQ,
                _ => throw new ArgumentNullException(null, nameof(locationType))
            };
        }

        public static string GetCatgeoryName(this EquipmentCategory equipmentCategory)
        {
            return equipmentCategory switch
            {
                EquipmentCategory.Rig => Business.EquipmentCategory.Rig,
                EquipmentCategory.Vehicles => Business.EquipmentCategory.Vehicles,
                EquipmentCategory.Trailers => Business.EquipmentCategory.Trailers,
                EquipmentCategory.Pumps => Business.EquipmentCategory.Pumps,
                EquipmentCategory.Tanks => Business.EquipmentCategory.Tanks,
                EquipmentCategory.Ancillary => Business.EquipmentCategory.Ancillary,
                _ => throw new ArgumentNullException(null, nameof(equipmentCategory))
            };
        }
      
        public static string GetServiceType(this ServiceType serviceType) 
        {
            return serviceType switch
            {
                ServiceType.RodsAndTubing => Business.ServiceType.RodsAndTubing,
                ServiceType.PAndA => Business.ServiceType.PAndA,
                ServiceType.Completion => Business.ServiceType.Completion,
                ServiceType.Yard => Business.ServiceType.Yard,
                ServiceType.Workovers => Business.ServiceType.Workovers,
                ServiceType.StandBy => Business.ServiceType.StandBy,
                _ => throw new ArgumentNullException(null, nameof(serviceType))
            };
        }
      
        public static string GetChargeType(this ChargeType chargeType)
        {
            return chargeType switch
            {
                ChargeType.EA => Business.ChargeTypes.EA,
                ChargeType.Hourly => Business.ChargeTypes.Hourly,
                ChargeType.Daily => Business.ChargeTypes.Daily,
                ChargeType.Gallon => Business.ChargeTypes.Gallon,
                _ => throw new ArgumentNullException(null, nameof(chargeType))
            };
        }
    }
}
