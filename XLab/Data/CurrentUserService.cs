using System.Security.Principal;
using XLab.Common.Interfaces;
using XLab.Web.ExtensionMethod;

namespace XLab.Web.Data;

public class CurrentUserService<TKey> : ICurrentUserService<TKey> where TKey : IConvertible
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal == null) return;

        UserId = claimsPrincipal.GetFromClaims<TKey>(Constants.AppClaims.Id);
        FullName = claimsPrincipal.GetFromClaims<string>(Constants.AppClaims.FullName);
        Principal = claimsPrincipal;
        IsAuthenticated = claimsPrincipal.Identity?.IsAuthenticated ?? false;
    }

    public TKey UserId { get; }
    public string FullName { get; }
    public IPrincipal Principal { get; set; }
    public bool IsAuthenticated { get; }
}