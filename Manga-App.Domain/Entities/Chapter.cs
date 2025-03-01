
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;
using System.Globalization;

namespace MangaApp.Domain.Entities;
public class Chapter : EntityAuditBase<Guid>
{
    public Guid MangaId { get; set; }
    public float Number { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Approved;//
    public Guid? ApprovedById { get; set; } = null; // Người duyệt
    public virtual AppUser? ApprovedBy { get; set; }
    public virtual Manga Manga { get; set; }
    private List<ChapterImage> _images;
    public IReadOnlyCollection<ChapterImage> Images => _images.AsReadOnly();

    public virtual ICollection<History> Histories {  get; set; }
    private Chapter()
    {
        _images = new List<ChapterImage>();
    }

    public void Approve()
    {
        ApprovalStatus = ApprovalStatus.Approved;
    }

    public void Reject()
    {
        ApprovalStatus = ApprovalStatus.Rejected;
    }

    public void ResetApproval()
    {
        ApprovalStatus = ApprovalStatus.Pending;
    }

    public static Chapter Create(Guid mangaId, float number, string title)
    {

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        // trim data 
        title = textInfo.ToTitleCase(title.ToLower().Trim());
        return new Chapter
        {
            MangaId = mangaId,
            Number = number,
            Title = title,
            Slug = title.ToSlug(),
            CreatedDate = DateTimeOffset.Now
        };
    }
    public void Update(float number, string title)
    {
        Number = number;
        Title = title;
        Slug = title.ToSlug();
        ModifiedDate = DateTimeOffset.Now;
    }

    public void SetChapterImages(List<ChapterImage>? images)
    {

        if (images == null || !images.Any()) return;

        // Cập nhật danh sách _images
        _images.Clear();
        _images.AddRange(images);
    }
}
