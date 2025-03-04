
using MangaApp.Domain.Abstractions.Entities;
using MangaApp.Domain.Abstractions;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Entities.Identity;

namespace MangaApp.Domain.Entities;

public class Comment : EntityBase<Guid>, IDateTracking
{
    public Guid UserId {  get; set; }
    public CommentAction Action { get; set; }
    public CommentType Type { get; set; }
    public Guid CommentTypeId { get; set; }
    public string Content {  get; set; }
    public Guid? ParentId { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }

    public virtual AppUser? User { get; set; }
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment>? Replies {  get; set; }

    public Comment(
        Guid userId,
        CommentAction action,
        CommentType type,
        Guid commentTypeId,
        string content,
        Guid? parentId
       )
    {
        UserId = userId;
        Action = action;
        Type = type;
        CommentTypeId = commentTypeId;
        Content = content;
        ParentId = parentId;
        CreatedDate = DateTimeOffset.UtcNow;
    }
}
