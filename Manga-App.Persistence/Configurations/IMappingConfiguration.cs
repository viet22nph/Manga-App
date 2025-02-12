using Microsoft.EntityFrameworkCore;

namespace MangaApp.Persistence.Configurations;

public interface IMappingConfiguration
{
    void ApplyConfiguration(ModelBuilder modelBuilder);
}
