using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MangaApp.Contract.Shares.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContentRating
{
    Safe,
    Suggestive,
    Erotica,
    Pornographic
}

