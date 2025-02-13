

namespace MangaApp.Contract.Security.Permissions;
public static partial class Permission
{
    public static class Manga
    {
        public const string All = "manga:*";
        public const string View = "manga:view";
        public const string Create = "manga:create";
        public const string Update = "manga:update";
        public const string Delete = "manga:delete";
        public const string Approve = "manga:approve";
    }
}