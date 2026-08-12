using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Security.JWT.FactoryExtensions
{
    public class CurrentUserHelper
    {
        public static void UpdateCurrentUserService(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.CurrentUserService"));
            foreach (var template in templates)
            {
                if (!template.OutputTarget.GetProject().HasMicrosoftNetSdkWeb())
                {
                    continue;
                }

                // Both Intent.Security.JWT and Intent.Security.MSAL call this helper independently.
                // Mark the file synchronously (before scheduling AfterBuild) so a second call for the
                // same template in the same run is a no-op, rather than relying on checks inside the
                // AfterBuild callback, which run in isolation and won't see each other's additions.
                if (template.CSharpFile.TryGetMetadata<bool>("current-user-service-wired-up", out _))
                {
                    continue;
                }
                template.CSharpFile.AddMetadata("current-user-service-wired-up", true);

                var currentUserTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Application.Identity.CurrentUserInterface"));
                if (currentUserTemplate == null)
                {
                    return;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("System.Security.Claims");
                    file.AddUsing("Duende.IdentityModel");
                    file.AddUsing("Microsoft.AspNetCore.Authorization");
                    file.AddUsing("Microsoft.AspNetCore.Http");

                    var priClass = file.Classes.First();

                    var ctor = priClass.Constructors.First();
                    if (!ctor.Parameters.Any(p => p.Name == "httpContextAccessor"))
                    {
                        ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor", p => p.IntroduceReadonlyField());
                    }

                    var contractedUserIdProperty = currentUserTemplate.CSharpFile.Interfaces.FirstOrDefault()?.Properties?.FirstOrDefault(p => p.Name == "Id");
                    if (contractedUserIdProperty is null)
                    {
                        throw new Exception($"Didn't find Id on {currentUserTemplate.ClassName}");
                    }

                    var userIdProperty = priClass.Properties.FirstOrDefault(p => p.Name == "UserId");
                    if (userIdProperty is not null)
                    {
                        userIdProperty
                            .WithoutSetter()
                            .Getter
                            .WithExpressionImplementation("GetUserId(GetClaimsPrincipal())");
                    }
                    var userNameProperty = priClass.Properties.FirstOrDefault(p => p.Name == "UserName");
                    if (userNameProperty is not null)
                    {
                        userNameProperty
                            .WithoutSetter()
                            .Getter
                            .WithExpressionImplementation("GetUserName(GetClaimsPrincipal())");
                    }

                    var getMethod = priClass.FindMethod("GetAsync");
                    if (getMethod is not null)
                    {
                        getMethod.Statements.Clear();

                        getMethod.AddStatement("var claimsPrincipal = GetClaimsPrincipal();");
                        getMethod.AddIfStatement("claimsPrincipal is null", ifs => ifs.AddStatement("return Task.FromResult((ICurrentUser?)null);"));
                        getMethod.AddStatement(@"ICurrentUser currentUser = new CurrentUser(
                    GetUserId(claimsPrincipal),
                    GetUserName(claimsPrincipal),
                    claimsPrincipal);", s => s.SeparatedFromPrevious());
                        getMethod.AddStatement("return Task.FromResult<ICurrentUser?>(currentUser);", s => s.SeparatedFromPrevious());
                    }

                    var authMethod = priClass.FindMethod("AuthorizeAsync");
                    if (authMethod is not null)
                    {
                        authMethod.Statements.Clear();
                        authMethod.Statements.Add(@"if (GetClaimsPrincipal() == null)
                    {
                        return false;
                    }");
                        authMethod.Statements.Add("");
                        authMethod.AddObjectInitStatement("var authService", " GetAuthorizationService();");
                        authMethod.AddIfStatement("authService == null", @if =>
                        {
                            @if.AddReturn("false");
                        });

                        authMethod.Statements.Add("");
                        authMethod.AddObjectInitStatement("var claimsPrinciple", "  GetClaimsPrincipal();");

                        authMethod.Statements.Add("return (await authService.AuthorizeAsync(claimsPrinciple!, policy))?.Succeeded ?? false;");

                    }
                    var roleMethod = priClass.FindMethod("IsInRoleAsync");
                    if (roleMethod is not null)
                    {
                        roleMethod.Statements.Clear();
                        roleMethod.Statements.Add("return await Task.FromResult(GetClaimsPrincipal()?.IsInRole(role) ?? false);");
                    }

                    if (priClass.FindMethod("GetClaimsPrincipal") is null)
                    {
                        priClass.AddMethod("ClaimsPrincipal?", "GetClaimsPrincipal", method =>
                        {
                            method.Private();
                            method.WithExpressionBody("_httpContextAccessor?.HttpContext?.User");
                        });
                    }

                    if (priClass.FindMethod("GetAuthorizationService") is null)
                    {
                        priClass.AddMethod("IAuthorizationService?", "GetAuthorizationService", method =>
                        {
                            method.Private();
                            method.WithExpressionBody("_httpContextAccessor?.HttpContext?.RequestServices.GetService(typeof(IAuthorizationService)) as IAuthorizationService");
                        });
                    }

                    if (priClass.FindMethod("GetUserName") is null)
                    {
                        priClass.AddMethod($"string?", "GetUserName", method =>
                        {
                            method
                                .Static()
                                .Private()
                                .AddParameter("ClaimsPrincipal?", "claimsPrincipal ");

                            method.WithExpressionBody("claimsPrincipal ?.FindFirst(JwtClaimTypes.Name)?.Value");
                        });
                    }

                    if (priClass.FindMethod("GetUserId") is null)
                    {
                        priClass.AddMethod(contractedUserIdProperty.Type.Replace("?", "") + "?", "GetUserId", method =>
                        {
                            method
                                .Static()
                                .Private()
                                .AddParameter("ClaimsPrincipal?", "claimsPrincipal ");

                            method.WithExpressionBody(GetUserIdImplimentation(contractedUserIdProperty));
                        });
                    }

                    if (!file.Records.Any(r => r.Name == "CurrentUser"))
                    {
                        file.AddRecord("CurrentUser", @record =>
                        {
                            record
                                .ImplementsInterface(template.GetTypeName(currentUserTemplate))
                                .AddPrimaryConstructor(ctor =>
                                {
                                    ctor
                                        .AddParameter(contractedUserIdProperty.Type.Replace("?", "") + "?", "Id")
                                        .AddParameter("string?", "Name")
                                        .AddParameter("ClaimsPrincipal", "Principal");
                                });
                        });
                    }
                });
            }
        }

        private static string GetUserIdImplimentation(CSharpProperty p)
        {
            var propertyType = p.Type.Replace("?", "");
            if (propertyType == "string")
            {
                return "claimsPrincipal?.FindFirst(JwtClaimTypes.Subject)?.Value";
            }
            else
            {
                return $"{propertyType}.TryParse(claimsPrincipal?.FindFirst(JwtClaimTypes.Subject)?.Value, out var parsed) ? parsed : default";
            }
        }

    }
}
