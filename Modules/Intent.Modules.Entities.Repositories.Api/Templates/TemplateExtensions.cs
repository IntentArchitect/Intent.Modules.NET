using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates
{
    public static class TemplateExtensions
    {
        public static string GetEntityRepositoryInterfaceName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetEntityRepositoryInterfaceName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, model);
        }

        public static string GetPagedResultInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PagedResultInterfaceTemplate.TemplateId);
        }

        public static string GetRepositoryInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}