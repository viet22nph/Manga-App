

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using Microsoft.AspNetCore.Http;

namespace MangaApp.Contract.Services.V1.Chapter;

public static class Command
{
    public record CreateChapterCommand(
        Guid MangaId,
        float Number,
        string Title,
        IFormFileCollection PageOrder
        ) : ICommand<Success>;
    public record UpdateChapterCommand(
        Guid Id,
        float Number,
        string Title,
        List<UpdateChapterImage> Images
        ) :ICommand<Success>;
   
}

public class UpdateChapterImage
{
    public string? Url { get; set; } // Ảnh cũ sẽ có URL
    public IFormFile? File { get; set; } // Ảnh mới sẽ có File
    public int Position { get; set; } // Vị trí sắp xếp ảnh
}