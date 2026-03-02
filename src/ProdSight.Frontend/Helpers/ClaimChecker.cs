using System.Security.Claims;

namespace ProdSight.Frontend.Helpers;

public static class ClaimChecker
{
    public static bool HasClaimValues(ClaimsPrincipal user, string claimType, IEnumerable<string> allowedValues)
    {
        if (user?.Identity?.IsAuthenticated != true) {return false;}
        if(String.IsNullOrWhiteSpace(claimType)){return false;}
        if(!allowedValues.Any()){return false;}
        
        var allowed = new HashSet<string>(allowedValues ?? [], StringComparer.OrdinalIgnoreCase);

        return user.FindAll(claimType).Any(claim => allowed.Contains(claim.Value));
    }
}