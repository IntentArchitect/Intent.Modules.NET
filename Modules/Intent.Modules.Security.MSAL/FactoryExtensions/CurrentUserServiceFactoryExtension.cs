using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CurrentUserServiceFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Security.MSAL.CurrentUserServiceFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.CurrentUserService"));
            if (template == null)
            {
                return;
            }
            template.AddNugetDependency(NugetPackages.IdentityModel(template.OutputTarget));
            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System.Security.Claims");
                file.AddUsing("IdentityModel");
                file.AddUsing("Microsoft.AspNetCore.Authorization");
                file.AddUsing("Microsoft.AspNetCore.Http");

                var priClass = file.Classes.First();
                priClass.AddField("IHttpContextAccessor", "_httpContextAccessor", prop => prop.PrivateReadOnly());

                var ctor = priClass.Constructors.First();
                ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor");
                ctor.AddStatement($@"_httpContextAccessor = httpContextAccessor;");

                var userIdProperty = priClass.Properties.First(p => p.Name == "UserId");
                userIdProperty
                    .WithoutSetter()
                    .Getter
                    .WithExpressionImplementation(GetUserIdImplimentation(userIdProperty));

                priClass.Properties.First(p => p.Name == "UserName")
                    .WithoutSetter()
                    .Getter
                    .WithExpressionImplementation("GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Name)?.Value");


                var authMethod = priClass.FindMethod("AuthorizeAsync");
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

                var roleMethod = priClass.FindMethod("IsInRoleAsync");
                roleMethod.Statements.Clear();
                roleMethod.Statements.Add("return await Task.FromResult(GetClaimsPrincipal()?.IsInRole(role) ?? false);");

                priClass.AddMethod("ClaimsPrincipal?", "GetClaimsPrincipal", method =>
                {
                    method.Private();
                    method.WithExpressionBody("_httpContextAccessor?.HttpContext?.User");
                });

                priClass.AddMethod("IAuthorizationService?", "GetAuthorizationService", method =>
                {
                    method.Private();
                    method.WithExpressionBody("_httpContextAccessor?.HttpContext?.RequestServices.GetService(typeof(IAuthorizationService)) as IAuthorizationService");
                });
            });
        }

        private string GetUserIdImplimentation(CSharpProperty p)
        {
            var propertyType = p.Type.Replace("?", "");
            if (propertyType == "string")
            {
                return "GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Subject)?.Value";
            }
            else
            {
                return $"{propertyType}.TryParse(GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Subject)?.Value, out var parsed) ? parsed : null";
            }
        }

    }
}