// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ExternalControllerTemplate : CSharpTemplateBase<object, Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController.ExternalAuthProviderDecorator>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly ");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderType));
            
            #line default
            #line hidden
            this.Write(" _");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderVariableName));
            
            #line default
            #line hidden
            this.Write(@";
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<ExternalController> _logger;
        private readonly IEventService _events;

        public ExternalController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger,
            ");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderVariableName));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            _");
            
            #line 47 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderVariableName));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 47 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderVariableName));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n            _interaction = interaction;\r\n            _clientStore = clientSt" +
                    "ore;\r\n            _logger = logger;\r\n            _events = events;\r\n        }\r\n\r" +
                    "\n        /// <summary>\r\n        /// initiate roundtrip to external authenticatio" +
                    "n provider\r\n        /// </summary>\r\n        [HttpGet]\r\n        public IActionRes" +
                    "ult Challenge(string scheme, string returnUrl)\r\n        {\r\n            if (strin" +
                    "g.IsNullOrEmpty(returnUrl)) returnUrl = \"~/\";\r\n\r\n            // validate returnU" +
                    "rl - either it is a valid OIDC URL or back to a local page\r\n            if (Url." +
                    "IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == fa" +
                    "lse)\r\n            {\r\n                // user might have clicked on a malicious l" +
                    "ink - should be logged\r\n                throw new Exception(\"invalid return URL\"" +
                    ");\r\n            }\r\n            \r\n            // start challenge and roundtrip th" +
                    "e return URL and scheme \r\n            var props = new AuthenticationProperties\r\n" +
                    "            {\r\n                RedirectUri = Url.Action(nameof(Callback)), \r\n   " +
                    "             Items =\r\n                {\r\n                    { \"returnUrl\", retu" +
                    "rnUrl }, \r\n                    { \"scheme\", scheme },\r\n                }\r\n       " +
                    "     };\r\n\r\n            return Challenge(props, scheme);\r\n            \r\n        }" +
                    "\r\n\r\n        /// <summary>\r\n        /// Post processing of external authenticatio" +
                    "n\r\n        /// </summary>\r\n        [HttpGet]\r\n        public async Task<IActionR" +
                    "esult> Callback()\r\n        {\r\n            // read external identity from the tem" +
                    "porary cookie\r\n            var result = await HttpContext.AuthenticateAsync(Iden" +
                    "tityServerConstants.ExternalCookieAuthenticationScheme);\r\n            if (result" +
                    "?.Succeeded != true)\r\n            {\r\n                throw new Exception(\"Extern" +
                    "al authentication error\");\r\n            }\r\n\r\n            if (_logger.IsEnabled(L" +
                    "ogLevel.Debug))\r\n            {\r\n                var externalClaims = result.Prin" +
                    "cipal.Claims.Select(c => $\"{c.Type}: {c.Value}\");\r\n                _logger.LogDe" +
                    "bug(\"External claims: {@claims}\", externalClaims);\r\n            }\r\n\r\n           " +
                    " // lookup our user and external provider info\r\n            var (user, provider," +
                    " providerUserId, claims) = await FindUserFromExternalProvider(result);\r\n        " +
                    "    if (user == null)\r\n            {\r\n                user = await CustomUserPro" +
                    "visionProcess(provider, providerUserId, claims);\r\n            }\r\n\r\n            /" +
                    "/ this allows us to collect any additional claims or properties\r\n            // " +
                    "for the specific protocols used and store them in the local auth cookie.\r\n      " +
                    "      // this is typically used to store data needed for signout from those prot" +
                    "ocols.\r\n            var additionalLocalClaims = new List<Claim>();\r\n            " +
                    "var localSignInProps = new AuthenticationProperties();\r\n            ProcessLogin" +
                    "Callback(result, additionalLocalClaims, localSignInProps);\r\n            \r\n      " +
                    "      ");
            
            #line 118 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetUserMappingCode()));
            
            #line default
            #line hidden
            this.Write(@"

            // issue authentication cookie for user
            var isuser = new IdentityServerUser(user_subjectId)
            {
                DisplayName = user_username,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items[""returnUrl""] ?? ""~/"";

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user_subjectId, user_username, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(""Redirect"", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(");
            
            #line 153 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderUserType));
            
            #line default
            #line hidden
            this.Write(@" user, string provider, string providerUserId, IEnumerable<Claim> claims)> FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception(""Unknown userid"");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items[""scheme""];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = ");
            
            #line 172 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetUserLookupCodeExpression()));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n            return (user, provider, providerUserId, claims);\r\n        }\r\n\r\n " +
                    "       ");
            
            #line 177 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAutoProvisionUserMethodCode()));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]\r\n        private Task" +
                    "<");
            
            #line 180 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.IdentityServer4.UI\Templates\Controllers\ExternalController\ExternalControllerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AuthProviderUserType));
            
            #line default
            #line hidden
            this.Write("> CustomUserProvisionProcess(string provider, string providerUserId, IEnumerable<" +
                    "Claim> claims)\r\n        {\r\n            // this might be where you might initiate" +
                    " a custom workflow for user registration\r\n            // in this sample we don\'t" +
                    " show how that would be done, as our sample implementation\r\n            // simpl" +
                    "y auto-provisions new external user\r\n            return AutoProvisionUser(provid" +
                    "er, providerUserId, claims);\r\n        }\r\n\r\n        // if the external login is O" +
                    "IDC-based, there are certain things we need to preserve to make logout work\r\n   " +
                    "     // this will be different for WS-Fed, SAML2p or other protocols\r\n        pr" +
                    "ivate void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> l" +
                    "ocalClaims, AuthenticationProperties localSignInProps)\r\n        {\r\n            /" +
                    "/ if the external system sent a session id claim, copy it over\r\n            // s" +
                    "o we can use it for single sign-out\r\n            var sid = externalResult.Princi" +
                    "pal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);\r\n            " +
                    "if (sid != null)\r\n            {\r\n                localClaims.Add(new Claim(JwtCl" +
                    "aimTypes.SessionId, sid.Value));\r\n            }\r\n\r\n            // if the externa" +
                    "l provider issued an id_token, we\'ll keep it for signout\r\n            var idToke" +
                    "n = externalResult.Properties.GetTokenValue(\"id_token\");\r\n            if (idToke" +
                    "n != null)\r\n            {\r\n                localSignInProps.StoreTokens(new[] { " +
                    "new AuthenticationToken { Name = \"id_token\", Value = idToken } });\r\n            " +
                    "}\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
