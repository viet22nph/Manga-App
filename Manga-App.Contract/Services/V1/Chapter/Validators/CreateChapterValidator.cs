

using FluentValidation;
using static MangaApp.Contract.Services.V1.Chapter.Command;

namespace MangaApp.Contract.Services.V1.Chapter.Validators;

public class CreateChapterValidator: AbstractValidator<CreateChapterCommand>
{
    private const long MaxFileSize = 2 * 1024 * 1024; //2mb
    public CreateChapterValidator()
    {
        RuleFor(x => x.MangaId)
            .NotEmpty().WithMessage("MangaId không được để trống.");

        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Số Chapter phải lớn hơn 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");

        RuleFor(x => x.PageOrder)
            .NotEmpty().WithMessage("Chapter phải có ít nhất một ảnh.")
            .Must(files => files.All(f => f.Length > 0)).WithMessage("Tất cả ảnh phải hợp lệ.")
            .Must(files => files.All(f => f.ContentType.StartsWith("image/"))).WithMessage("Tất cả file phải là ảnh.")
            .Must(files => files.Count <= 500).WithMessage("Không thể tải lên quá 500 ảnh trong một Chapter.")
            .Must(files => files.All(f => f.Length <= MaxFileSize))
            .WithMessage($"Mỗi ảnh không được vượt quá {MaxFileSize / (1024 * 1024)}MB.");
    }
}
