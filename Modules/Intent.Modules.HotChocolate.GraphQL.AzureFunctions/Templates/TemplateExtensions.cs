using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.AzureFunctions.Templates.GraphQLConfig;
using Intent.Modules.HotChocolate.GraphQL.AzureFunctions.Templates.GraphQLFunction;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetGraphQLConfigTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(GraphQLConfigTemplate.TemplateId);
        }

        public static string GetGraphQLFunctionTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(GraphQLFunctionTemplate.TemplateId);
        }

    }
}