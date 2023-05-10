using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates.GraphQLConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetGraphQLConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(GraphQLConfigurationTemplate.TemplateId);
        }

    }
}