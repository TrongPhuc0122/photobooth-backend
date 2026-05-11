using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Voucher
    {
        [Key]
        public int VoucherId{ get; set; }
        public string VoucherCode{ get; set; } = string.Empty;
        public string Purpose{ get; set; } = string.Empty;
        public float DiscountPercent{ get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;
        public DateTime StartDate{ get; set; } = DateTime.UtcNow;
        public DateTime EndDate{ get; set; } = DateTime.UtcNow;
        public int UsageLimit{ get; set; }
        public int UsageCount { get; set; }
    }
}