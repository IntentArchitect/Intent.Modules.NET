using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMutationResolverName<T>(this IIntentTemplate<T> template) where T : Intent.Modules.Modelers.Services.GraphQL.Api.GraphQLMutationTypeModel
        {
            return template.GetTypeName(MutationResolverTemplate.TemplateId, template.Model);
        }

        public static string GetMutationResolverName(this IIntentTemplate template, Intent.Modules.Modelers.Services.GraphQL.Api.GraphQLMutationTypeModel model)
        {
            return template.GetTypeName(MutationResolverTemplate.TemplateId, model);
        }

        public static string GetQueryResolverName<T>(this IIntentTemplate<T> template) where T : Intent.Modules.Modelers.Services.GraphQL.Api.GraphQLQueryTypeModel
        {
            return template.GetTypeName(QueryResolverTemplate.TemplateId, template.Model);
        }

        public static string GetQueryResolverName(this IIntentTemplate template, Intent.Modules.Modelers.Services.GraphQL.Api.GraphQLQueryTypeModel model)
        {
            return template.GetTypeName(QueryResolverTemplate.TemplateId, model);
        }

    }
}