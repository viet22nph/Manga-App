namespace MangaApp.Contract.Security.Permissions;

public static partial class Permission
{
    public static class User
    {
        public const string All = "user:*";
        public const string View = "user:view";
        public const string Create = "user:create";
        public const string Update = "user:update";
        public const string Ban = "user:ban";
        public const string Unban = "user:unban";
        public const string AssignRole = "user:assign-role";
    }
}

