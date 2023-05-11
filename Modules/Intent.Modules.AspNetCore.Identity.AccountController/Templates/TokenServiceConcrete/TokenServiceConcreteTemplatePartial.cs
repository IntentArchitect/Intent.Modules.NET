using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.TokenServiceConcrete
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class TokenServiceConcreteTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.TokenServiceConcrete";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TokenServiceConcreteTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Security.Cryptography")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.IdentityModel.Tokens")
                .AddClass($"TokenService", @class =>
                {
                    @class.ImplementsInterface(this.GetTokenServiceInterfaceName());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                    @class.AddMethod("string", "GenerateAccessToken", method =>
                    {
                        method.AddParameter("string", "username");
                        method.AddParameter("IEnumerable<Claim>", "claims");
                        method.AddStatement(new CSharpObjectInitializerBlock("var tokenClaims = new List<Claim>")
                            .AddStatement("new Claim(JwtRegisteredClaimNames.Sub, username)")
                            .AddStatement("new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())")
                            .WithSemicolon());

                        method.AddStatement("tokenClaims.AddRange(claims);");
                        method.AddStatements($@"
var signingKey = Convert.FromBase64String(_configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!);
var issuer = _configuration.GetSection(""JwtToken:Issuer"").Get<string>()!;
var audience = _configuration.GetSection(""JwtToken:Audience"").Get<string>()!;
var expiration = TimeSpan.FromMinutes(_configuration.GetSection(""JwtToken:AuthTokenExpiryMinutes"").Get<int?>() ?? 120);");
                        method.AddStatement(new CSharpInvocationStatement("var token = new JwtSecurityToken")
                            .AddArgument("issuer: issuer")
                            .AddArgument("audience: audience")
                            .AddArgument("expires: DateTime.UtcNow.Add(expiration)")
                            .AddArgument("claims: tokenClaims")
                            .AddArgument(@"signingCredentials: new SigningCredentials(key: new SymmetricSecurityKey(signingKey), algorithm: SecurityAlgorithms.HmacSha256)")
                            .WithArgumentsOnNewLines());
                        method.AddStatement($@"return new JwtSecurityTokenHandler().WriteToken(token);");
                    });
                    @class.AddMethod("(string Token, DateTime Expiry)", "GenerateRefreshToken", method =>
                    {
                        method.AddStatement("var randomNumber = new byte[32];");
                        method.AddUsingBlock("var rng = RandomNumberGenerator.Create()", block => block
                            .AddStatement("rng.GetBytes(randomNumber);")
                            .AddStatement(@"return (Convert.ToBase64String(randomNumber), DateTime.Now.AddDays(_configuration.GetSection(""JwtToken:RefreshTokenExpiryMinutes"").Get<int?>() ?? 3));"));
                    });
                    @class.AddMethod("ClaimsPrincipal", "GetPrincipalFromExpiredToken", method =>
                    {
                        method.AddParameter("string", "token");
                        method.AddObjectInitializerBlock(
                            "var tokenValidationParameters = new TokenValidationParameters", block => block
                                .AddInitStatement("ValidateAudience", "true")
                                .AddInitStatement("ValidAudience",
                                    @"_configuration.GetSection(""JwtToken:Audience"").Get<string>()")
                                .AddInitStatement("ValidateIssuer", "true")
                                .AddInitStatement("ValidIssuer",
                                    @"_configuration.GetSection(""JwtToken:Issuer"").Get<string>()")
                                .AddInitStatement("ValidateIssuerSigningKey", "true")
                                .AddInitStatement("IssuerSigningKey",
                                    @"new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!))")
                                .AddInitStatement("ValidateLifetime", "false")
                                .AddInitStatement("NameClaimType", @"""sub""")
                                .WithSemicolon());
                        method.AddStatements(@"
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;");
                        method.AddIfStatement("jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)", stmt => stmt
                            .AddStatement(@"throw new SecurityTokenException(""Invalid token"");"));
                        method.AddStatement("return principal;");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}