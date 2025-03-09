

using MangaApp.Domain.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System;
using MangaApp.Contract.Shares.Enums;

namespace MangaApp.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>, IDateTracking, ISoftDelete
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string DisplayName { get; set; }
    public string? Avatar { get; set; }
    public Gender? Gender { get; set; }


    public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
    public virtual ICollection<IdentityUserToken<Guid>> UserTokens { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }

    public virtual ICollection<Manga>? MangaApproved { get; set; } // manga đã duyệt
    public virtual ICollection<Chapter>? ChapterApproved { get; set; }
    public virtual ICollection<AppUserRole> UserRoles { get; set; }
    public virtual ICollection<Follow> Follows { get; set; }
    public virtual ICollection<History> Histories { get; set; }
    public virtual ICollection<Rating> Ratings { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<Notification> ReceiveNotifications { get; set; }
    public virtual ICollection<Notification> SenderNotifications { get; set; }
    private AppUser()
    {
        Claims = new List<IdentityUserClaim<Guid>>();
        Logins = new List<IdentityUserLogin<Guid>>();
        UserTokens = new List<IdentityUserToken<Guid>>();
        UserRoles = new List<AppUserRole>();
    }
    public static AppUser CreateNewUser(string email, string userName)
    {
        return new AppUser
        {
            Email = email,
            UserName = userName,
            DisplayName = userName,
        };
    }

}