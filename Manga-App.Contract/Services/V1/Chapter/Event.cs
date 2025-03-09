
using MangaApp.Contract.Abstractions.IntegrationEvents;

namespace MangaApp.Contract.Services.V1.Chapter;

public static  class Event
{
    public class NewChapterCreatedConsumer
    {
        public Guid ChapterId { get; set; }
        public Guid MangaId { get; set; }
        public string Title { get; set; }
        public string MangaName { get; set; }
        public string MangaSlug { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
