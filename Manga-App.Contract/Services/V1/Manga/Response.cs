
using MangaApp.Contract.Dtos.Chapter;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;

namespace MangaApp.Contract.Services.V1.Manga;

public static class Response
{
    public class MangaResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? AnotherTitle { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string Thumbnail { get; set; }
        public string Slug { get; set; }
        public CountryDto Country { get; set; }
        public string Status { get; set; }
        public string ContentRating { get; set; }
        public int? Year { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string ApprovalStatus { get; set; }
        public string State { get; set; }
        public List<GenreDto> Genres { get; set; }
        public ChapterDto? LatestUploadedChapter { get; set; }
    };

    public class MangaDetailResponse
    {
        public MangaResponse Manga {  get; set; }
        public long View { get; set; }
        public float Rating { get; set; }
        public int Follow { get; set; }
    };
}
