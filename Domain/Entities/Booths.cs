using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Commons;

namespace Domain.Entities
{
    public class Booths : BaseEntity
    {
        [Key]
        public Guid BoothId{ get; set; }
        public string BoothIp{ get; set; } = string.Empty;
        public string BoothName{ get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int BranchId{ get; set; }
        public int? SettingId { get; set; }
        public int? SettingHistoryId { get; set; }
        public string? Creator { get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;

        [ForeignKey("BranchId")]
        public virtual Branch Branch{ get; set; } = null!;
        [ForeignKey("SettingId")]
        public virtual Setting? Setting { get; set; }
        [ForeignKey("SettingHistoryId")]
        public virtual SettingHistory? SettingHistory { get; set; }

        public virtual BoothHealth? BoothHealth { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public virtual BoothResources? BoothResources { get; set; }
        public virtual ICollection<BoothError> BoothErrors { get; set; } = new List<BoothError>();
        public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}