using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Modules.HotChocolate.GraphQL.Templates.SubscriptionType;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMutationTypeName<T>(this IIntentTemplate<T> template) where T : Intent.Modules.HotChocolate.GraphQL.Models.IGraphQLMutationTypeModel
        {
            return template.GetTypeName(MutationTypeTemplate.TemplateId, template.Model);
        }

        public static string GetMutationTypeName(this IIntentTemplate template, Intent.Modules.HotChocolate.GraphQL.Models.IGraphQLMutationTypeModel model)
        {
            return template.GetTypeName(MutationTypeTemplate.TemplateId, model);
        }

        public static string GetQueryTypeName<T>(this IIntentTemplate<T> template) where T : Intent.Modules.HotChocolate.GraphQL.Models.IGraphQLQueryTypeModel
        {
            return template.GetTypeName(QueryTypeTemplate.TemplateId, template.Model);
        }

        public static string GetQueryTypeName(this IIntentTemplate template, Intent.Modules.HotChocolate.GraphQL.Models.IGraphQLQueryTypeModel model)
        {
            return template.GetTypeName(QueryTypeTemplate.TemplateId, model);
        }

        public static string GetSubscriptionTypeName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Services.GraphQL.Api.GraphQLSubscriptionTypeModel
        {
            return template.GetTypeName(SubscriptionTypeTemplate.TemplateId, template.Model);
        }

        public static string GetSubscriptionTypeName(this IIntentTemplate template, Intent.Modelers.Services.GraphQL.Api.GraphQLSubscriptionTypeModel model)
        {
            return template.GetTypeName(SubscriptionTypeTemplate.TemplateId, model);
        }

    }
}