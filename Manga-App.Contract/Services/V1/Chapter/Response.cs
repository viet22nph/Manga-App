
using System.Runtime.InteropServices;

namespace MangaApp.Contract.Services.V1.Chapter;

public class Response
{
    public record ChapterResponse(
        Guid Id,
        string Type,
        ChapterAttributes Attributes
        );
  
    public record ChapterDetailReponse(
        Guid Id,
        string Type,
        ChapterAttributesDetail Attributes

    );
   
}
public record ChapterAttributes(
      float Number,
      Guid MangaId,
      string Title,
      DateTimeOffset CreatedAt,
      DateTimeOffset? UpdatedAt,
      int Pages
  );
public record ChapterAttributesDetail(
       float Number,
       Guid MangaId,
       string Title,
       List<string> Url,
       DateTimeOffset CreatedAt,
       DateTimeOffset? UpdatedAt,
       int Pages
   );