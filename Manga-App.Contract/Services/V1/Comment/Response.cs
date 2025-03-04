
using MangaApp.Contract.Shares.Enums;
using RabbitMQ.Client;
using System.Net.Mime;

namespace MangaApp.Contract.Services.V1.Comment;

public static class Response
{
    public class CommentResponse
    {
        public Guid Id {  get; set; }
        public CommentType Type { get; set; }
        public CommentAction Action {  get; set; }
        public Guid TargetId {  get; set; }
        public CommentUser Author { get; set; }
        public string Content {  get; set; }
        public int ReplyCount {  get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt {  get; set; }

    }

    public class CommentUser
    {
        public Guid Id {  get; set; }
        public string DisplayName {  get; set; }
        public string Email {  get; set; }
         public string? Avatar {  get; set; }
    }
    public class ReplyResponse
    {
        public Guid Id { get; set; }
        public CommentType Type { get; set; }
        public CommentAction Action { get; set; }
        public Guid TargetId { get; set; }
        public Guid ParentId { get; set; }
        public CommentUser Author { get; set; }
        public string Content { get; set; }
        public int ReplyCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
