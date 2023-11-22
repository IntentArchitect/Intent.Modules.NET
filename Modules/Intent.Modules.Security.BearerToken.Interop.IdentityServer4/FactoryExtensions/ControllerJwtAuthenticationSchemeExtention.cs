using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ControllerJwtAuthenticationSchemeExtention : FactoryExtensionBase
    {
        public override string Id => "Intent.Security.BearerToken.Interop.IdentityServer4.ControllerJwtAuthenticationSchemeExtention";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // These arguments are required to make JWT bearer auth work with AspNetCore.
            // Setting the default AuthenticationScheme scheme in the startup configuration doesn't work for some reason.
            // I suspect that the default AuthenticationScheme is not being respected when you run Identity Server alongside JWT auth.
            // https://stackoverflow.com/questions/40646312/asp-net-core-authorize-attribute-not-working-with-jwt
            var controllers = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.WebApi.Controller));
            foreach (var controller in controllers)
            {
                controller.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var authorizeAttr = @class.Attributes.FirstOrDefault(x => x.Name == "Authorize");
                    authorizeAttr?.AddArgument($"AuthenticationSchemes = {controller.UseType("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults")}.AuthenticationScheme");

                    foreach (var method in @class.Methods)
                    {
                        authorizeAttr = method.Attributes.FirstOrDefault(x => x.Name == "Authorize");
                        authorizeAttr?.AddArgument($"AuthenticationSchemes = {controller.UseType("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults")}.AuthenticationScheme");
                    }
                }, int.MaxValue);
            }
        }
    }
}