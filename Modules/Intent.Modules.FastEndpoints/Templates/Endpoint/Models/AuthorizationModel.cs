using System;
using System.Linq;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class AuthorizationModel : IAuthorizationModel
{
    public static AuthorizationModel FromRolesAndPolicies(string? roles, string? policies)
    {
        return new AuthorizationModel
        {
            RolesExpression = !string.IsNullOrWhiteSpace(roles)
                ? @$"{string.Join("+", roles.Split('+', StringSplitOptions.RemoveEmptyEntries)
                    .Select(group => string.Join(",", group.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))))}"
                : null,
            Policy = !string.IsNullOrWhiteSpace(policies)
                ? @$"{string.Join("+", policies.Split('+', StringSplitOptions.RemoveEmptyEntries)
                    .Select(group => string.Join(",", group.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))))}"
                : null
        };
    }
    
    ///<summary>
    /// Gets or sets the policy name that determines access to the resource. Note the format will generate exactly in C#.
    ///</summary>
    public string? Policy { get; set; }


    ///<summary>
    /// Gets or sets the Roles that determines access to this Resource. Note the format will generate exactly in C#.
    ///</summary>
    public string? RolesExpression { get; set; }
}