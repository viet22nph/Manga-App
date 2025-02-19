using MangaApp.Domain.Abstractions;

namespace MangaApp.Domain.Entities;

public class Country: EntityBase<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    private readonly List<Manga> _manga;
    public IReadOnlyCollection<Manga> Manga => _manga.AsReadOnly();

    private Country()
    {
        _manga = new List<Manga>();
    }
    public Country(string countryCode, string countryName)
    {
        Code = !string.IsNullOrWhiteSpace(countryCode) ? countryCode : throw new ArgumentNullException(nameof(Code));
        Name = !string.IsNullOrWhiteSpace(countryName) ? countryName : throw new ArgumentNullException(nameof(Name));
    }
}
