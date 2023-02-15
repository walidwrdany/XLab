using Microsoft.AspNetCore.Identity;
using XLab.Common.Interfaces;

namespace XLab.Web.Data.Entities;

public class User : IdentityUser<int>, IEntity<int>
{
    public User()
    {
        Claims = new HashSet<UserClaim>();
        Logins = new HashSet<UserLogin>();
        Tokens = new HashSet<UserToken>();
        UserRoles = new HashSet<UserRole>();
    }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserPassword { get; set; }
    public bool IsAdministrator { get; set; }

    public ICollection<UserClaim> Claims { get; set; }
    public ICollection<UserLogin> Logins { get; set; }
    public ICollection<UserToken> Tokens { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}