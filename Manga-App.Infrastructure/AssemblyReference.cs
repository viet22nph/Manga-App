using System.Reflection;

namespace Manga_App.Infrastructure;
public class AssemblyReference
{

    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}