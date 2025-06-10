using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IdentityService.Templates.TokenServiceInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.TokenService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TokenServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.TokenService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TokenServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Security.Cryptography")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.AspNetCore.DataProtection")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.IdentityModel.Tokens")
                .AddClass($"TokenService", @class =>
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("JwtToken:Issuer", "https://localhost:{sts_port}"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("JwtToken:Audience", "api"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("JwtToken:SigningKey", "aHHDYCTvyZVbdcGgaDvL+T6837pHCkciU0rLvUbE9a4="));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("JwtToken:AuthTokenExpiryTimeSpan", "02:00:00"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("JwtToken:RefreshTokenExpiryTimeSpan", "3.00:00:00"));

                    @class.ImplementsInterface(this.GetTokenServiceInterfaceName());

                    @class.AddField("IDataProtector", "_protector", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IDataProtectionProvider", "provider");
                        ctor.AddStatement($"_protector = provider.CreateProtector(\"{CSharpFile!.Namespace}.{@class.Name}\");");
                    });

                    @class.AddMethod("(string Token, DateTime Expiry)", "GenerateAccessToken", method =>
                    {
                        method.AddParameter("string", "username");
                        method.AddParameter("IList<Claim>", "claims");

                        method.AddIfStatement("claims.Any(p => p.Type == JwtRegisteredClaimNames.Name)", stmt =>
                        {
                            stmt.AddStatement(@"throw new ArgumentException($""Claim '{JwtRegisteredClaimNames.Name}' is reserved. Ensure that the correct name is passed through the 'username' parameter."");");
                        });

                        method.AddStatement(new CSharpObjectInitializerBlock("var tokenClaims = new List<Claim>")
                            .AddStatement("new Claim(JwtRegisteredClaimNames.Name, username)")
                            .AddStatement("new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())")
                            .WithSemicolon()
                            .SeparatedFromPrevious());

                        method.AddIfStatement("claims.All(p => p.Type != JwtRegisteredClaimNames.Sub)", stmt =>
                        {
                            stmt.AddStatement(@"throw new ArgumentException($""The '{JwtRegisteredClaimNames.Sub}' claim has not been set. Ensure you add it in the 'claims' List parameter."");");
                            stmt.SeparatedFromPrevious();
                        });

                        method.AddStatement("tokenClaims.AddRange(claims);", stmt => stmt.SeparatedFromPrevious());
                        method.AddStatements($@"
                            var signingKey = Convert.FromBase64String(_configuration.GetSection(""JwtToken:SigningKey"").Get<string>()!);
                            var issuer = _configuration.GetSection(""JwtToken:Issuer"").Get<string>()!;
                            var audience = _configuration.GetSection(""JwtToken:Audience"").Get<string>()!;
                            var expires = DateTime.UtcNow.Add(_configuration.GetSection(""JwtToken:AuthTokenExpiryTimeSpan"").Get<TimeSpan?>() ?? TimeSpan.FromMinutes(120));");
                        method.AddStatement(new CSharpInvocationStatement("var token = new JwtSecurityToken")
                            .AddArgument("issuer: issuer")
                            .AddArgument("audience: audience")
                            .AddArgument("expires: expires")
                            .AddArgument("claims: tokenClaims")
                            .AddArgument(@"signingCredentials: new SigningCredentials(key: new SymmetricSecurityKey(signingKey), algorithm: SecurityAlgorithms.HmacSha256)")
                            .WithArgumentsOnNewLines());
                        method.AddStatement($@"return (new JwtSecurityTokenHandler().WriteToken(token), expires);");
                    });

                    @class.AddMethod("(string Token, DateTime Expiry)", "GenerateRefreshToken", method =>
                    {
                        method.AddParameter("string", "username");
                        method.AddStatements(@"
                            var expiry = DateTime.UtcNow.Add(_configuration.GetSection(""JwtToken:RefreshTokenExpiryTimeSpan"").Get<TimeSpan?>() ?? TimeSpan.FromDays(3));
                            var unprotected = JsonSerializer.Serialize(new RefreshToken { Username = username, Expiry = expiry });
                            var token = _protector.Protect(unprotected);

                            return (token, expiry);".ConvertToStatements());
                    });

                    @class.AddMethod("string?", "GetUsernameFromRefreshToken", method =>
                    {
                        method.AddParameter("string?", "token");
                        method.AddStatements(@"
                            if (token == null)
                            {
                                return null;
                            }

                            try
                            {
                                var unprotected = _protector.Unprotect(token);
                                var decoded = JsonSerializer.Deserialize<RefreshToken>(unprotected);
                                if (decoded == null)
                                {
                                    return null;
                                }

                                if (DateTime.UtcNow >= decoded.Expiry)
                                {
                                    return null;
                                }

                                return decoded.Username;
                            }
                            catch (CryptographicException)
                            {
                                return null;
                            }".ConvertToStatements());
                    });

                    @class.AddNestedClass("RefreshToken", nestedClass =>
                    {
                        nestedClass.Private();
                        nestedClass.AddProperty("string?", "Username");
                        nestedClass.AddProperty("DateTime", "Expiry");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) return;
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this).ForInterface(GetTemplate<TokenServiceInterfaceTemplate>(TokenServiceInterfaceTemplate.TemplateId)));
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