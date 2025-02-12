
using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;

namespace MangaApp.Domain.Entities;
public class Chapter : EntityAuditBase<Guid>
{
    public Guid MangaId { get; set; }
    public float Number { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Volume { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;// chờ được duyệt
    public Guid? ApprovedById { get; set; } = null; // Người duyệt
    public virtual AppUser? ApprovedBy { get; set; }
    public virtual Manga Manga { get; set; }
    private List<ChapterImage> _images;
    public IReadOnlyCollection<ChapterImage> Images => _images.AsReadOnly();
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
}
