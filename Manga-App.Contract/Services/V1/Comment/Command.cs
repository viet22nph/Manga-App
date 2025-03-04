
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Services.V1.Comment;

public static class Command
{
    public record CreateCommentCommand(string Content, Guid UserId, CommentType Type, Guid TypeId , Guid? ParentId) : ICommand<Success>;
    //public record RemoveCommentCommand(Guid id): ICommand<Success>;
}
