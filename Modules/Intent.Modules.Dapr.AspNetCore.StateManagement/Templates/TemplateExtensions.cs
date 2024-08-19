using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepository;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepositoryInterface;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepository;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepositoryBase;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepositoryInterface;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreUnitOfWork;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDaprStateStoreGenericRepositoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreGenericRepositoryTemplate.TemplateId);
        }

        public static string GetDaprStateStoreGenericRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreGenericRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetDaprStateStoreRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DaprStateStoreRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetDaprStateStoreRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DaprStateStoreRepositoryTemplate.TemplateId, model);
        }

        public static string GetDaprStateStoreRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreRepositoryBaseTemplate.TemplateId);
        }

        public static string GetDaprStateStoreRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetDaprStateStoreUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreUnitOfWorkTemplate.TemplateId);
        }

        public static string GetDaprStateStoreUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprStateStoreUnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}