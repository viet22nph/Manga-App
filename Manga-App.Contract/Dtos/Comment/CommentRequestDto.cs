

using MangaApp.Contract.Shares.Enums;

namespace MangaApp.Contract.Dtos.Comment;

public class CommentRequestDto
{
    public CommentType CommentType { get; set; }
    public Guid TypeId { get; set; }
    public string Content {  get; set; }
    public Guid? ParentId { get; set; }
}
