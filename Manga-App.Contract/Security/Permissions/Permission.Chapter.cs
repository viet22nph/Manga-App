
namespace MangaApp.Contract.Security.Permissions;

public static partial class Permission
{
    public static class Chapter
    {
        public const string All = "chapter:*";
        public const string View = "chapter:view";
        public const string Create = "chapter:create";
        public const string Update = "chapter:update";
        public const string Delete = "chapter:delete";
        public const string Approve = "chapter:approve";
    }
}