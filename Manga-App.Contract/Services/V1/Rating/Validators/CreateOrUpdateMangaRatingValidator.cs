
using FluentValidation;
using static MangaApp.Contract.Services.V1.Rating.Command;

namespace MangaApp.Contract.Services.V1.Rating.Validators;

public class CreateOrUpdateMangaRatingValidator:AbstractValidator<CreateOrUpdateMangaRatingCommand>
{
    public CreateOrUpdateMangaRatingValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .WithMessage("Rating must be between 1 and 10.");
    }
}
