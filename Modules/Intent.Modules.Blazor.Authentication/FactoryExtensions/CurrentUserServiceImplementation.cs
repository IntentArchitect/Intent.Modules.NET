using System;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CurrentUserServiceImplementation : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Authentication.CurrentUserServiceImplementation";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 100;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {            
            AdjustCurrentUserService(application);
        }

        private void AdjustCurrentUserService(IApplication application)
        {
            if (application.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly())
            {
                return;
            }

            var securityConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration"));
            if (securityConfigTemplate != null)
            {
                securityConfigTemplate.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("ConfigureApplicationSecurity");
                    var statement = method?.FindStatement(s => s.GetText("").StartsWith("services.AddSingleton<ICurrentUserService"));
                    if (statement != null)
                    {
                        statement.Replace(statement.GetText("").Replace("AddSingleton", "AddScoped"));
                    }
                }, 100);
            }            

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.CurrentUserService"));
            if (template == null)
            {
                return;
            }

            var currentUserTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Application.Identity.CurrentUserInterface"));
            if (currentUserTemplate == null)
            {
                return;
            }


            template.CSharpFile.AfterBuild(file =>
            {
                var userIdProperty = currentUserTemplate.CSharpFile.Interfaces.FirstOrDefault()?.Properties?.FirstOrDefault(p => p.Name == "Id");

                file.AddUsing("System.Security.Claims");
                file.AddUsing("Microsoft.AspNetCore.Authorization");
                file.AddUsing("Microsoft.AspNetCore.Components.Authorization");

                var @class = file.Classes.First();
                @class.ImplementsInterface(template.GetSetUserContextInterfaceName());

                @class.AddField("ClaimsPrincipal?", "_cachedUser", f => f.Private());

                var ctor = @class.Constructors.First();
                if (!ctor.Parameters.Any(p => p.Name == "httpContextAccessor"))
                {
                    ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor", p => p.IntroduceReadonlyField());
                }
                if (!ctor.Parameters.Any(p => p.Name == "authStateProvider"))
                {
                    ctor.AddParameter("AuthenticationStateProvider", "authStateProvider", p => p.IntroduceReadonlyField());
                }
                if (!ctor.Parameters.Any(p => p.Name == "authorizationService"))
                {
                    ctor.AddParameter("IAuthorizationService", "authorizationService", p => p.IntroduceReadonlyField());
                }

                var roleMethod = @class.FindMethod("IsInRoleAsync");
                roleMethod.Statements.Clear();

                roleMethod.AddStatement("var user = await GetPrincipalAsync();");
                roleMethod.AddStatement("return user?.IsInRole(role) ?? false;");

                var authMethod = @class.FindMethod("AuthorizeAsync");
                authMethod.Statements.Clear();

                authMethod.AddStatement("var user = await GetPrincipalAsync();");
                authMethod.AddIfStatement("user is null", ifs => ifs.AddStatement("return false;"));
                authMethod.AddStatement("var result = await _authorizationService.AuthorizeAsync(user, policy);");
                authMethod.AddStatement("return result.Succeeded;");

                var getMethod = @class.FindMethod("GetAsync");
                getMethod.Async();
                getMethod.Statements.Clear();

                getMethod.AddStatement("var user = await GetPrincipalAsync();");
                getMethod.AddIfStatement("user is null || !(user?.Identity?.IsAuthenticated == true)", ifs => ifs.AddStatement("return null;"));
                getMethod.AddStatement("return new CurrentUser(GetUserId(user), GetUserName(user), user);");

                @class.AddMethod("void", "SetContext", method =>
                {
                    method
                        .IsExplicitImplementationFor("ISetUserContext")
                        .AddParameter("ClaimsPrincipal", "principal");

                    method.WithExpressionBody("_cachedUser = principal");
                });

                @class.AddMethod("ClaimsPrincipal?", "GetPrincipalAsync", method =>
                {
                    method
                        .Async()
                        .Private();

                    method.AddIfStatement("_cachedUser is not null", ifs => ifs.AddStatement("return _cachedUser;"));
                    method.AddStatement("var httpUser = _httpContextAccessor.HttpContext?.User;", s => s.SeparatedFromPrevious());

                    method.AddIfStatement("httpUser?.Identity is not null", ifs =>
                    {
                        ifs.AddIfStatement("httpUser?.Identity?.IsAuthenticated == true", ifs => ifs.AddStatement("_cachedUser = httpUser;"));
                        ifs.AddElseStatement(e =>
                        {
                            e.AddStatement("return null;");
                        });
                    });
                    method.AddElseStatement(e =>
                    {
                        e.AddStatement("var authState = await _authStateProvider.GetAuthenticationStateAsync();");
                        e.AddStatement("_cachedUser = authState.User;");
                    });

                    method.AddStatement("return _cachedUser;", s => s.SeparatedFromPrevious());
                });

                var getUserNameMethod = @class.FindMethod("GetUserName");
                if (getUserNameMethod is null)
                {
                    @class.AddMethod($"string?", "GetUserName", method =>
                    {
                        method
                            .Static()
                            .Private()
                            .AddParameter("ClaimsPrincipal?", "claimsPrincipal");

                        method.WithExpressionBody("claimsPrincipal?.Identity?.Name");
                    });

                }
                else
                {
                    getUserNameMethod.Statements.Clear();
                    getUserNameMethod.WithExpressionBody("claimsPrincipal?.Identity?.Name");
                }


                var getUserIdMethod = @class.FindMethod("GetUserId");
                if (getUserIdMethod is null)
                {
                    @class.AddMethod(userIdProperty.Type.Replace("?", "") + "?", "GetUserId", method =>
                    {
                        method
                            .Static()
                            .Private()
                            .AddParameter("ClaimsPrincipal?", "claimsPrincipal");

                        method.WithExpressionBody(GetUserIdImplimentation(userIdProperty));
                    });
                }
                else
                {
                    getUserIdMethod.Statements.Clear();
                    getUserIdMethod.WithExpressionBody(GetUserIdImplimentation(userIdProperty));
                }

                var toRemove = @class.FindMethod("GetClaimsPrincipal");
                if (toRemove is not null)
                {
                    @class.Methods.Remove(toRemove);
                }
                toRemove = @class.FindMethod("GetAuthorizationService");
                if (toRemove is not null)
                {
                    @class.Methods.Remove(toRemove);
                }


                var syncUserIdProperty = @class.Properties.FirstOrDefault(p => p.Name == "UserId");
                if (syncUserIdProperty is not null)
                {
                    syncUserIdProperty
                        .WithoutSetter()
                        .Getter
                        .WithExpressionImplementation(@"_httpContextAccessor?.HttpContext?.User is null 
            ? null 
            : GetUserId(_httpContextAccessor.HttpContext.User)");
                }
                var syncUserNameProperty = @class.Properties.FirstOrDefault(p => p.Name == "UserName");
                if (syncUserNameProperty is not null)
                {
                    syncUserNameProperty
                        .WithoutSetter()
                        .Getter
                        .WithExpressionImplementation(@"_httpContextAccessor?.HttpContext?.User is null 
            ? null : 
            GetUserName(_httpContextAccessor.HttpContext.User)");
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
                                    .AddParameter(userIdProperty.Type.Replace("?", "") + "?", "Id")
                                    .AddParameter("string?", "Name")
                                    .AddParameter("ClaimsPrincipal", "Principal");
                            });
                    });
                }
            });
        }

        private string GetUserIdImplimentation(CSharpProperty p)
        {
            var propertyType = p.Type.Replace("?", "");
            if (propertyType == "string")
            {
                return "claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value";
            }
            else
            {
                return $"{propertyType}.TryParse(claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsed) ? parsed : default";
            }
        }
    }
}