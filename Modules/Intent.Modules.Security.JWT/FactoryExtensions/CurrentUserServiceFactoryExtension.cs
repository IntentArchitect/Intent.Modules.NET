using System;
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

namespace Intent.Modules.Security.JWT.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CurrentUserServiceFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Security.JWT.CurrentUserServiceFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.CurrentUserService"));
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System.Security.Claims");
                file.AddUsing("IdentityModel");
                file.AddUsing("Microsoft.AspNetCore.Authorization");
                file.AddUsing("Microsoft.AspNetCore.Http");

                var priClass = file.Classes.First();
                priClass.AddField("ClaimsPrincipal?", "_claimsPrincipal", prop => prop.PrivateReadOnly());
                var ctor = priClass.Constructors.First();
                ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor");
                ctor.AddStatement($@"_claimsPrincipal = httpContextAccessor?.HttpContext?.User;");
                ctor.AddParameter("IAuthorizationService", "authorizationService", prop => prop.IntroduceReadonlyField());
                var userIdProperty = priClass.Properties.First(p => p.Name == "UserId");
                userIdProperty
                    .WithoutSetter()
                    .Getter
                    .WithExpressionImplementation(GetUserIdImplimentation(userIdProperty));
                priClass.Properties.First(p => p.Name == "UserName")
                    .WithoutSetter()
                    .Getter
                    .WithExpressionImplementation("_claimsPrincipal?.FindFirst(JwtClaimTypes.Name)?.Value");

                var authMethod = priClass.FindMethod("AuthorizeAsync");
                authMethod.Statements.Clear();
                authMethod.Statements.Add("if (_claimsPrincipal == null) return false;");
                authMethod.Statements.Add("return (await _authorizationService.AuthorizeAsync(_claimsPrincipal, policy)).Succeeded;");

                var roleMethod = priClass.FindMethod("IsInRoleAsync");
                roleMethod.Statements.Clear();
                roleMethod.Statements.Add("return await Task.FromResult(_claimsPrincipal?.IsInRole(role) ?? false);");
            });
        }

        private string GetUserIdImplimentation(CSharpProperty p)
        {
            var propertyType = p.Type.Replace("?", "");
            if (propertyType == "string")
            {
                return "_claimsPrincipal?.FindFirst(JwtClaimTypes.Subject)?.Value";
            }
            else
            {
                return $"{propertyType}.TryParse(_claimsPrincipal?.FindFirst(JwtClaimTypes.Subject)?.Value, out var parsed) ? parsed : null";
            }
        }
    }
}