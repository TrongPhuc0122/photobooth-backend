using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class PaymentNotification
{
    [Key]
    public int NotificationId { get; set; }

    // Của BE (EPAY không gửi)
    public DateTime ReceivedAt { get; set; }
    public int? PaymentId { get; set; }

    // 14 trường từ mục 6.6.2
    [MaxLength(50)] public string RequestId { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; }
    public DateTime? BankTranTime { get; set; }
    [MaxLength(50)] public string ReferenceId { get; set; } = string.Empty;
    [MaxLength(50)] public string MapId { get; set; } = string.Empty;
    public Decimal Amount { get; set; }
    [MaxLength(200)] public string Signature { get; set; } = string.Empty;
    [MaxLength(50)] public string MerchantCode { get; set; } = string.Empty;
    public Decimal Fee { get; set; }
    [MaxLength(50)] public string VaName { get; set; } = string.Empty;
    [MaxLength(50)] public string VaAcc { get; set; } = string.Empty;
    [MaxLength(50)] public string? BankCode { get; set; }
    [MaxLength(50)] public string? BankName { get; set; }
    [MaxLength(500)] public string? Remark { get; set; }
}