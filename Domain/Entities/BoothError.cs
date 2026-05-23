using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BoothError
    {
        [Key]
        public int ErrorId{ get; set; }
        public Guid BoothId{ get; set; }
        public string ErrorCode{ get; set; } = string.Empty;
        public string Cause { get; set; } = string.Empty;
        public string? Solution { get; set; }
        public bool IsFixed{ get; set; }
        public string? ResolvedBy { get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;
        [ForeignKey("BoothId")]
        public virtual Booths? Booths { get; set; }
    }
}