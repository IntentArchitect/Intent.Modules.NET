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
        public static string GetMutationResolverName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.ServiceModel
        {
            return template.GetTypeName(MutationResolverTemplate.TemplateId, template.Model);
        }

        public static string GetMutationResolverName(this IntentTemplateBase template, Intent.Modelers.Services.Api.ServiceModel model)
        {
            return template.GetTypeName(MutationResolverTemplate.TemplateId, model);
        }

        public static string GetQueryResolverName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.ServiceModel
        {
            return template.GetTypeName(QueryResolverTemplate.TemplateId, template.Model);
        }

        public static string GetQueryResolverName(this IntentTemplateBase template, Intent.Modelers.Services.Api.ServiceModel model)
        {
            return template.GetTypeName(QueryResolverTemplate.TemplateId, model);
        }

    }
}