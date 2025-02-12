
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace MangaApp.Persistence.Configurations;

public class MappingEntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : class
{
    public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Các cấu hình cho TEntity được chỉ định ở đây
    }
}
