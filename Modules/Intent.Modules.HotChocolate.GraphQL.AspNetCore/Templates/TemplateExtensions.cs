using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates.ConfigureGraphQL;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetConfigureGraphQLName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ConfigureGraphQLTemplate.TemplateId);
        }

    }
}