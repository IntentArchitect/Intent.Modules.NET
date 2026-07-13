using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CosmosDB.MultiTenancy.SeperateDB.Api
{
    // TEMPORARY, LOCAL-ONLY test shim for Finbuckle/CosmosDB runtime verification.
    // Not part of any Intent template output - not committed. Accepts "Authorization: Bearer test:<subject>"
    // and maps it to a ClaimsPrincipal so Basic Auditing (which requires a UserId claim) can be exercised
    // without standing up a real OIDC STS.
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var value = authHeader.ToString();
            const string prefix = "Bearer test:";
            if (!value.StartsWith(prefix))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var subject = value.Substring(prefix.Length);
            var claims = new[] { new Claim("sub", subject), new Claim("name", subject) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
