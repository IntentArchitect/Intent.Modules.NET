using System.Collections.Generic;
using Intent.Modules.AzureFunctions.Jwt.Templates.CurrentUser;
using Intent.Modules.AzureFunctions.Jwt.Templates.JwtClaimsMiddleware;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Jwt.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCurrentUserName(this IIntentTemplate template)
        {
            return template.GetTypeName(CurrentUserTemplate.TemplateId);
        }
        public static string GetJwtClaimsMiddlewareName(this IIntentTemplate template)
        {
            return template.GetTypeName(JwtClaimsMiddlewareTemplate.TemplateId);
        }

    }
}