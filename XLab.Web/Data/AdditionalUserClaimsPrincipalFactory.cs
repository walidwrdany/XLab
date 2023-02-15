using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using XLab.Web.Data.Entities;

namespace XLab.Web.Data;

public class AdditionalUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<User, Role>
{
    private readonly UserManager<User> _userManager;

    public AdditionalUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var principal = await base.CreateAsync(user);
        if (principal.Identity is not ClaimsIdentity identity) return principal;

        identity.AddClaim(new Claim(Constants.AppClaims.Id, user.Id.ToString()));
        identity.AddClaim(new Claim(Constants.AppClaims.FullName, user.FullName));

        return principal;
    }
}