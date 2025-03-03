
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;
using System.Diagnostics.Metrics;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

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


    public virtual ICollection<Follow> Follows {  get; set; }
    public virtual ICollection<History> Histories { get; set; }
    public virtual ICollection<Rating> Ratings { get; set; }
    public virtual List<MangaViews> Views { get; set; }
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
        int? year,
        bool? IsPublished = false 
    )
    {

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        // trim data 
        title = textInfo.ToTitleCase(title.ToLower().Trim()); 
        anotherTitle = 
            string.IsNullOrEmpty(anotherTitle) ? anotherTitle: textInfo.ToTitleCase(anotherTitle!.ToLower().Trim());
        author = string.IsNullOrEmpty(description) ? description :textInfo.ToTitleCase(description!.ToLower().Trim());
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
            IsPublished = IsPublished?? false,
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
                MangaId = this.Id, // Assuming the manga entity has an Id
                GenreId = genreId
            });
        }
    }

    public void Update(
        string title,
        string? anotherTitle,
        string? description,
        string? author,
        string thumbnail,
        int countryId,
        MangaStatus status,
        ContentRating contentRating,
        string slug,
        int? year)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        // trim data 
        title = textInfo.ToTitleCase(title.ToLower().Trim());
        anotherTitle =
            string.IsNullOrEmpty(anotherTitle) ? anotherTitle : textInfo.ToTitleCase(anotherTitle!.ToLower().Trim());
        author = string.IsNullOrEmpty(description) ? description : textInfo.ToTitleCase(description!.ToLower().Trim());
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
        ModifiedDate = DateTimeOffset.UtcNow;

    }
    public Result<Success> Approve(Guid approvedId)
    {
        if(ApprovalStatus == ApprovalStatus.Approved)
        {
            return Error.Failure(code: nameof(ApprovalStatus.Approved), description: "Manga đã được duyệt trước đó.");
        }
        ApprovalStatus = ApprovalStatus.Approved;
        ApprovedById = approvedId;
        ModifiedDate = DateTimeOffset.UtcNow;
        return ResultType.Success;
    }

    public Result<Success> Reject(Guid approvedId) {

        if (ApprovalStatus == ApprovalStatus.Rejected)
        {
            return Error.Failure(code: nameof(ApprovalStatus.Rejected), description: "Manga đã được hủy trước đó.");
        }
        IsDeleted = true;
        ApprovalStatus = ApprovalStatus.Rejected;
        ApprovedById = approvedId;
        ModifiedDate = DateTimeOffset.UtcNow;
        return ResultType.Success;
    }

    public Result<Success> Publish()
    {
        if (ApprovalStatus != ApprovalStatus.Approved)
        {
            return Error.Failure(code: nameof(IsPublished), description: "Manga chưa được duyệt, không thể xuất bản..");
        }
        if (ApprovalStatus == ApprovalStatus.Rejected)
        {
            return Error.Failure(code: nameof(IsPublished), description: "Manga đã bị hủy, không thể xuất bản..");
        }
        if (IsPublished)
        {
            return Error.Failure(code: nameof(IsPublished), description: "Manga đã được xuất bản trước đó.");
        }

        IsPublished = true;
        ModifiedDate = DateTimeOffset.UtcNow;
        return ResultType.Success;
    }
    public Result<Success> UnPublish()
    {
        if (!IsPublished)
        {
            return Error.Failure(code: nameof(IsPublished), description: "Manga chưa được xuất bản.");
        }

        IsPublished = false;
        ModifiedDate = DateTimeOffset.UtcNow;
        return ResultType.Success;
    }

}
