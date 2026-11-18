using Shared;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public Guid BoothId { get; set; }
    public Booths? Booth { get; set; }

    [MaxLength(50)]
    public string InvoiceCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    [MaxLength(50)]
    public string map_id { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? account_no { get; set; }
    public string? qr_rawdata { get; set; }

    public ICollection<PaymentNotification> Notifications { get; set; } = new List<PaymentNotification>();
}