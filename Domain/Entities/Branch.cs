using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Shared;

namespace Domain.Entities
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PhoneNumber { get; set; } = string.Empty;

        public virtual ICollection<Booths> Booths { get; set; } = new List<Booths>();
    }
}