
namespace MangaApp.Persistence.Constants;

public static class TableNames
{
    // identity table name
    internal const string AppUser = nameof(AppUser);
    internal const string AppRole = nameof(AppRole);
    internal const string AppUserRole = nameof(AppUserRole);
    internal const string AppRoleClaim = nameof(AppRoleClaim);
    internal const string AppUserClaim = nameof(AppUserClaim);
    internal const string AppUserToken = nameof(AppUserToken);
    internal const string AppUserLogin = nameof(AppUserLogin);

    // ****** Singular nouns ********

    internal const string Manga = nameof(Manga);
    internal const string Chapter = nameof(Chapter);
    internal const string ChapterImage = nameof(ChapterImage);
    internal const string Country = nameof(Country);
    internal const string MangaGenre = nameof(MangaGenre);
    internal const string Genre = nameof(Genre);
}