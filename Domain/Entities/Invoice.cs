using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;

        public int BoothId { get; set; }

        public int? VoucherId { get; set; }  

        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public bool FlowStatus { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===================== NAVIGATION =====================

        [ForeignKey(nameof(BoothId))]
        public virtual Booths? Booth { get; set; }

        [ForeignKey(nameof(VoucherId))]
        public virtual Voucher? Voucher { get; set; }
    }
}