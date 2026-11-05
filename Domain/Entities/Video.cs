using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared;

namespace Domain.Entities;

public class Video
{
    [Key]
    public int VideoId { get; set; }
    public Guid BoothId { get; set; }
    public DateTime CreateAt { get; set; }

    [ForeignKey(nameof(BoothId))]
    public virtual Booths? Booth { get; set; }
}