using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared;

namespace Domain.Entities
{
    public class BoothHealth
    {
        [Key]
        public Guid BoothId{ get; set; }
        public Status Status{ get; set; }
        //public Status CameraStatus { get; set; }
        //public Status PrinterStatus { get; set; }
        public DateTime LastHeartbeat{ get; set;} = DateTime.UtcNow;   
        [ForeignKey("BoothId")]
        public virtual Booths? Booths { get; set; }
    }
}
