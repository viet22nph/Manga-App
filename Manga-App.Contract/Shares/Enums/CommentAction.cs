

using System.Text.Json.Serialization;

namespace MangaApp.Contract.Shares.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CommentAction
    {
        Commented,
        Replied
    }
}
