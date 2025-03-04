

using FluentValidation;
using static MangaApp.Contract.Services.V1.Comment.Command;

namespace MangaApp.Contract.Services.V1.Comment.Validators;
public class CreateCommentValidator: AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content)
                     .NotEmpty().WithMessage("Nội dung không được để trống.")
                     .MinimumLength(1).WithMessage("Nội dung phải có ít nhất 1 ký tự.")
                     .MaximumLength(1000).WithMessage("Nội dung không được quá 1000 ký tự.");

        RuleFor(x => x.TypeId)
            .NotEmpty().WithMessage("TypeId không được để trống.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("TypeId không hợp lệ.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId không được để trống.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("UserId không hợp lệ.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Loại bình luận không hợp lệ.");
    }
}

