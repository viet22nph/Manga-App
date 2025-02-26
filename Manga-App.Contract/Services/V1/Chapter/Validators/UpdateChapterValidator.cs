
using FluentValidation;
using static MangaApp.Contract.Services.V1.Chapter.Command;

namespace MangaApp.Contract.Services.V1.Chapter.Validators;

public class UpdateChapterValidator: AbstractValidator<UpdateChapterCommand>
{
    public UpdateChapterValidator()
    {
        RuleFor(x => x.Id)
          .NotEmpty().WithMessage("Chapter id không được để trống.");

        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Số Chapter phải lớn hơn 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");
    }
}
