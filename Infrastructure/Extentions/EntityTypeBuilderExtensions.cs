using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Extentions;

public static class EntityTypeBuilderExtensions
{
    public static void OwnsSettingGroups<T>(this EntityTypeBuilder<T> builder) where T : class, IHasSettingGroups
    {
        builder.OwnsOne(x => x.Camera);
        builder.OwnsOne(x => x.Printer);
        builder.OwnsOne(x => x.System, sys =>
        {
            sys.OwnsOne(x => x.Default);
            sys.OwnsOne(x => x.Timer);
            sys.OwnsOne(x => x.Capture);
            sys.OwnsOne(x => x.Audio);
        });
    }
}