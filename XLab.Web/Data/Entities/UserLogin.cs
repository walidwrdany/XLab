using Microsoft.AspNetCore.Identity;

namespace XLab.Web.Data.Entities;

public class UserLogin : IdentityUserLogin<int>
{
    public User User { get; set; }
}