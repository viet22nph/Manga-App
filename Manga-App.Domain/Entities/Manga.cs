
using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;
using System.Diagnostics.Metrics;

namespace MangaApp.Domain.Entities;


public class Manga : EntityAuditBase<Guid>
{
    public string Title { get; set; } // tên truyện
    public string? AnotherTitle { get; set; } // tên khác
    public string? Description { get; set; } // mô tả ngắn
    public string? Author { get; set; } // tác giả
    public string Thumbnail { get; set; } // ảnh bìa
    public string Slug { get; set; }// đừng dẫn url
    public int CountryId { get; set; }
    public MangaStatus Status { get; set; }
    public ContentRating ContentRating { get; set; }
    public int? Year { get; private set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeleteAt { get; set; }

    public virtual Country? Country { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    public bool IsPublished { get; set; } = false;

    public Guid? ApprovedById { get; set; } = null; // Người duyệt
    public virtual AppUser? ApprovedBy { get; set; }

    private List<MangaGenre> _mangaGenres;
    public IReadOnlyCollection<MangaGenre> MangaGenres => _mangaGenres.AsReadOnly();


    private List<Chapter> _chapters;
    public IReadOnlyCollection<Chapter> Chapters => _chapters.AsReadOnly();

    private Manga()
    {
        _mangaGenres = new List<MangaGenre>();
        _chapters = new List<Chapter>();
    }
    public Manga(string title, string? anotherTitle, string? description, string? author, string thumbnail, string slug, int countryId, MangaStatus status, ContentRating contentRating, int? year) : this()
    {
        Title = title;
        AnotherTitle = anotherTitle;
        Description = description;
        Author = author;
        Thumbnail = thumbnail;
        Slug = slug;
        CountryId = countryId;
        Status = status;
        ContentRating = contentRating;
        Year = year;

    }

    public static Manga Create(
        string title,
        string? anotherTitle,
        string? description,
        string? author,
        string thumbnail,
        int countryId,
        MangaStatus status,
        ContentRating contentRating,
        string slug,
        int? year
    )
    {
        // trim data
        title = title.Trim();
        anotherTitle = anotherTitle?.Trim();
        author = author?.Trim();
        thumbnail = thumbnail.Trim();
        return new Manga
        {
            Title = title,
            AnotherTitle = anotherTitle,
            Description = description,
            Author = author,
            Thumbnail = thumbnail,
            Slug = slug,
            CountryId = countryId,
            Status = status,
            ContentRating = contentRating,
            Year = year,
            CreatedDate = DateTimeOffset.UtcNow,
            ApprovalStatus = ApprovalStatus.Pending,
            IsPublished = false,
        };
    }
    public void SetStoryGenresByGenreIds(List<int>? genreIds)
    {
        if (genreIds == null)
            return;

        // Clear existing genres
        _mangaGenres.Clear();

        // Add new genres based on the provided IDs
        foreach (var genreId in genreIds)
        {
            _mangaGenres.Add(new MangaGenre
            {
                MangaId = this.Id, // Assuming the Story entity has an Id
                GenreId = genreId
            });
        }
    }
}
