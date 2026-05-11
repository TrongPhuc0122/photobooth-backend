using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class BoothResources
    {
        [Key]
        public int BoothId{ get; set; }
        public int PaperCount{ get; set; }
        public int PaperMax { get; set; }
        public int RibbonCount{ get; set; }
        public int RibbonMax { get; set; }
        public DateTime LastUpdated{ get; set; } = DateTime.UtcNow;
        [ForeignKey("BoothId")]
        public virtual Booths? Booths { get; set; }
        
    }
}