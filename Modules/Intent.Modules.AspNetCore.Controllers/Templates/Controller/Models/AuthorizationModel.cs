namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

public class AuthorizationModel : IAuthorizationModel
{
    ///<summary>
    /// Gets or sets the Authentication Schemes that determines access to this resource. Note the format will generate exactly in C#.
    ///</summary>
    public string AuthenticationSchemesExpression { get; set; }

    ///<summary>
    /// Gets or sets the policy name that determines access to the resource. Note the format will generate exactly in C#.
    ///</summary>
    public string Policy { get; set; }


    ///<summary>
    /// Gets or sets the Roles that determines access to this Resource. Note the format will generate exactly in C#.
    ///</summary>
    public string RolesExpression { get; set; }
}