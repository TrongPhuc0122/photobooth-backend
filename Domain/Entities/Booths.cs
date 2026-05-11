using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Booths
    {
        [Key]
        public int BoothId{ get; set; }
        public string BoothIp{ get; set; } = string.Empty;
        public string BoothName{ get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int BranchId{ get; set; }
        public string? Creator { get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;

        [ForeignKey("BranchId")]
        public virtual Branch? Branch{ get; set; }

        public virtual BoothHealth? BoothHealth { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public virtual BoothResources? BoothResources { get; set; }
        public virtual ICollection<BoothError> BoothErrors { get; set; } = new List<BoothError>();
    }
}