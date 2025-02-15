using System.Reflection;

namespace MangaApp.Presentation;
public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
