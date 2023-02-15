using Microsoft.AspNetCore.Identity;

namespace XLab.Web.Data.Entities;

public class UserClaim : IdentityUserClaim<int>
{
    public User User { get; set; }
}