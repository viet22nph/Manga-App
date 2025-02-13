using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace MangaApp.Contract.Extensions;

public static class StringExtension
{
    /// <summary>
    /// Convert the string to a slug (friendly URL format).
    /// </summary>
    /// <param name="phrase">The input chain needs to be switched.</param>
    /// <returns>Standardized slug strings.</returns>
    public static string ToSlug(this string phrase)
    {
        Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        string slug = phrase.Normalize(NormalizationForm.FormD).Trim().ToLower();

        slug = regex.Replace(slug, String.Empty)
          .Replace('\u0111', 'd').Replace('\u0110', 'D')
          .Replace(",", "-").Replace(".", "-").Replace("!", "")
          .Replace("(", "").Replace(")", "").Replace(";", "-")
          .Replace("/", "-").Replace("%", "ptram").Replace("&", "va")
          .Replace("?", "").Replace('"', '-').Replace(' ', '-');

        return slug;
    }
    public static string RemoveDiacritics(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
