using System.Reflection;

namespace Manga_App.Domain;

public class AssemblyReference
{

    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}