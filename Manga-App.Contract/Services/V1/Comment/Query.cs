
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using static MangaApp.Contract.Services.V1.Comment.Response;

namespace MangaApp.Contract.Services.V1.Comment;

public static class Query
{
    public record GetCommentQuery(CommentType Type, Guid TargetId,  int PageIndex, int PageSize): 
        IQuery<Pagination<CommentResponse>>;
    public record GetRepliesQuery(CommentType Type, Guid TargetId,Guid ParentId ,int PageIndex, int PageSize) : IQuery<Pagination<ReplyResponse>>;
}
