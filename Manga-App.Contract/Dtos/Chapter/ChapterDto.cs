
namespace MangaApp.Contract.Dtos.Chapter;

public class ChapterDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset ReleaseDate {  get; set; }
}
