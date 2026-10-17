using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class SettingLog
{
    [Key]
    public int SettingLogId { get; set; }
    public Guid BoothId { get; set; }
    public int SettingId { get; set; }
    public DateTime SettingTime { get; set; }  
    public bool SettingStatus { get; set; }   
    public DateTime CreateAt { get; set; }

    public virtual Booths Booth { get; set; } = null!;
    public virtual Setting Setting { get; set; } = null!;
}