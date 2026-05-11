using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared;

namespace Domain.Entities
{
    public class BoothHealth
    {
        [Key]
        public int BoothId{ get; set; }
        public Status Status{ get; set; }
        public DateTime LastHeartbeat{ get; set;} = DateTime.UtcNow;   
        [ForeignKey("BoothId")]
        public virtual Booths? Booths { get; set; }
    }
}
