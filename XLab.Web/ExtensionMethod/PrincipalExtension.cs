using System.Security.Claims;
using System.Security.Principal;

namespace XLab.Web.ExtensionMethod;

public static class PrincipalExtension
{
    public static TResult GetFromClaims<TResult>(this IPrincipal principal, string claimsName)
        where TResult : IConvertible
    {
        var value = ((ClaimsPrincipal) principal).FindFirst(x => x.Type == claimsName);
        if (value == null) return default;

        try
        {
            return (TResult) Convert.ChangeType(value.Value, typeof(TResult));
        }
        catch (Exception)
        {
            return default;
        }
    }
}