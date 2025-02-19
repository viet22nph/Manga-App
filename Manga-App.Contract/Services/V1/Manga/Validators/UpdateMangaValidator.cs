

using FluentValidation;
using static MangaApp.Contract.Services.V1.Manga.Command;

namespace MangaApp.Contract.Services.V1.Manga.Validators;

public class UpdateMangaValidator : AbstractValidator<UpdateMangaCommand>
{
    public UpdateMangaValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Title)
            .NotNull()
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(x => x.CountryId).NotNull();
        RuleFor(x => x.Thumbnail).NotNull();
        RuleFor(x => x.Status)
           .IsInEnum().WithMessage("Status không hợp lệ");
        RuleFor(x => x.ContentRating)
            .IsInEnum().WithMessage("ContentRating không hợp lệ");
        RuleFor(x => x.GenresId)
           .NotEmpty()
           .Must(genres => genres.All(id => id > 0)).WithMessage("GenresId phải chứa các ID hợp lệ");
    }

}
