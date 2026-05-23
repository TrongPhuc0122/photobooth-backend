using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BoothResources> BoothResources { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Booths> Booths { get; set; }
        public DbSet<BoothHealth> BoothHealth { get; set; }
        public DbSet<BoothError> BoothError { get; set; }
       
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
    
}