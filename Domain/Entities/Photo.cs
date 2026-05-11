using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Photo
    {
        [Key]
        public int PhotoId{ get; set; }
        public int BoothId{ get; set; }
        public string ImageUrl{ get; set; } = string.Empty;
        public int PrintCount{ get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.UtcNow;

        [ForeignKey("BoothId")]
        public virtual Booths? Booths   { get; set; }
    }
}