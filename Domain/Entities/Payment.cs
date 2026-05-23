using Shared;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Payment
{
    [Key]
    public int PaymentId { get; set; }
    public decimal Price { get; set; }
    public PaymentMethodStatus Method { get; set; }
    public string TransactionRef { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}