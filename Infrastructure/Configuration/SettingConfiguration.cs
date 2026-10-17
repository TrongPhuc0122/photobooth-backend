using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(s => s.SettingId);

        builder.OwnsOne(s => s.Camera, b => b.ToJson());
        builder.OwnsOne(s => s.Printer, b => b.ToJson());

        builder.OwnsOne(s => s.System, sys =>
        {
            sys.ToJson();
            sys.OwnsOne(x => x.Default);
            sys.OwnsOne(x => x.Timer);
            sys.OwnsOne(x => x.Capture);
            sys.OwnsOne(x => x.Audio);
        });

        builder.HasMany(s => s.Booths)
               .WithOne(b => b.Setting)
               .HasForeignKey(b => b.SettingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}