using Microsoft.AspNetCore.Identity;

namespace XLab.Web.Data.Entities;

public class RoleClaim : IdentityRoleClaim<int>
{
    public Role Role { get; set; }
}