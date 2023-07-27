using System.Collections.Generic;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Templates.AuthorizeCheckOperationFilter;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Security.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuthorizeCheckOperationFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthorizeCheckOperationFilterTemplate.TemplateId);
        }

    }
}