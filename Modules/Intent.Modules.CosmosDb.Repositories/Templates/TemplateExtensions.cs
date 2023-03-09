using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDb.Repositories.Templates.CosmosRepositoryBase;
using Intent.Modules.CosmosDb.Repositories.Templates.PagedList;
using Intent.Modules.CosmosDb.Repositories.Templates.Repository;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Repositories.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCosmosRepositoryBaseName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CosmosRepositoryBaseTemplate.TemplateId);
        }

        public static string GetPagedListName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PagedListTemplate.TemplateId);
        }

        public static string GetRepositoryName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetRepositoryName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, model);
        }

    }
}