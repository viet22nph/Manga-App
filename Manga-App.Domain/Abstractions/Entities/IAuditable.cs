
namespace MangaApp.Domain.Abstractions.Entities;

public interface IAuditable : ISoftDelete, IUserTracking, IDateTracking
{
}