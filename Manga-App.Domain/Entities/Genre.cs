
using MangaApp.Contract.Extensions;
using MangaApp.Domain.Abstractions;

namespace MangaApp.Domain.Entities;

public class Genre : EntityBase<int>
{

    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }

    private List<MangaGenre> _mangaGenres;
    public IReadOnlyCollection<MangaGenre> MangaGenres => _mangaGenres.AsReadOnly();
    private Genre()
    {
        _mangaGenres = new List<MangaGenre>();

    }
    public Genre(string name, string description) : this()
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentNullException(nameof(Description));
        Slug = name.ToSlug();
    }
}