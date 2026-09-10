using Domain.Entities;
using Domain.Entities.Commons;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeleted = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var deletedAt = Expression.Property(parameter, nameof(BaseEntity.DeletedAt));

                var active = Expression.AndAlso(
                    Expression.Equal(isDeleted, Expression.Constant(false)),
                    Expression.Equal(deletedAt, Expression.Constant(null, typeof(DateTime?))));

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(active, parameter));
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.Entity<Setting>().OwnsOne(s => s.Camera);
            modelBuilder.Entity<Setting>().OwnsOne(s => s.Printer);
        }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BoothResources> BoothResources { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Booths> Booths { get; set; }
        public DbSet<BoothHealth> BoothHealth { get; set; }
        public DbSet<BoothError> BoothError { get; set; }
        public DbSet<Frame> Frames { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
    
}