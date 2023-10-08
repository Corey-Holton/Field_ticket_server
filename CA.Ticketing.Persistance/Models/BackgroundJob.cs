using CA.Ticketing.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Ticketing.Persistance.Models
{
    [Table(TableNames.BackgroundJobs)]
    public class BackgroundJob
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Schedule { get; set; } = string.Empty;

        public DateTime? LastRunOn { get; set; }
    }
}
