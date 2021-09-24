using System.Collections.Generic;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.AuthorizeCheckOperationFilter;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuthorizeCheckOperationFilterName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AuthorizeCheckOperationFilterTemplate.TemplateId);
        }

        public static string GetSwashbuckleConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(SwashbuckleConfigurationTemplate.TemplateId);
        }

    }
}