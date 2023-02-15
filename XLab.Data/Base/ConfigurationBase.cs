using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XLab.Common.Interfaces;

namespace XLab.Data.Base;

public abstract class ConfigurationBase<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditable
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntity(builder);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(e => e.IsActive);
        builder.Property(e => e.CreatedBy).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime");
        builder.Property(e => e.ModifiedBy).IsRequired(false);
        builder.Property(e => e.ModifiedAt).HasColumnType("datetime");
    }

    public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}