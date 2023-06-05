using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepository;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.EFRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCustomRepositoryName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Repositories.Api.RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryName(this IIntentTemplate template, Intent.Modelers.Domain.Repositories.Api.RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, model);
        }

        public static string GetCustomRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Repositories.Api.RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryInterfaceName(this IIntentTemplate template, Intent.Modelers.Domain.Repositories.Api.RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, model);
        }

        public static string GetEFRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EFRepositoryInterfaceTemplate.TemplateId);
        }
        public static string GetPagedListName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedListTemplate.TemplateId);
        }

        public static string GetRepositoryName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetRepositoryName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, model);
        }

        public static string GetRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(RepositoryBaseTemplate.TemplateId);
        }

    }
}