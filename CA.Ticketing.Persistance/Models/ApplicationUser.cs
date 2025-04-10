﻿using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string DisplayName => $"{FirstName} {LastName}";

        [ForeignKey(nameof(Employee))]
        public string? EmployeeId { get; set; }

        [JsonIgnore]
        public virtual Employee? Employee { get; set; }

        [ForeignKey(nameof(CustomerContact))] 
        public string? CustomerContactId { get; set; }

        [JsonIgnore]
        public virtual CustomerContact? CustomerContact { get; set; }

        public string TicketIdentifier { get; set; } = string.Empty;

        public string Signature { get; set; } = string.Empty;

        public DateTime LastModifiedDate { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }
}
