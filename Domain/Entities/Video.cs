using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared;

namespace Domain.Entities;

public class Video
{
    [Key]
    public int VideoId { get; set; }

    public Guid BoothId { get; set; }

    public DateTime CreatedAt { get; set; }

    [MaxLength(50)]
    public string InvoiceCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public string VideoPath { get; set; } = string.Empty;

    [ForeignKey(nameof(BoothId))]
    public virtual Booths? Booth { get; set; }
}