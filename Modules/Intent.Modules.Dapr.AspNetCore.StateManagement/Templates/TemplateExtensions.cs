using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.GenericStateStoreRepository;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.GenericStateStoreRepositoryInterface;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateStoreRepository;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateStoreRepositoryUnitOfWorkBehaviour;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateStoreUnitOfWork;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateStoreUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates
{
    public static class TemplateExtensions
    {
        public static string GetGenericStateStoreRepositoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(GenericStateStoreRepositoryTemplate.TemplateId);
        }

        public static string GetGenericStateStoreRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(GenericStateStoreRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetStateStoreRepositoryName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(StateStoreRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetStateStoreRepositoryName(this IIntentTemplate template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(StateStoreRepositoryTemplate.TemplateId, model);
        }

        public static string GetStateStoreRepositoryUnitOfWorkBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateStoreRepositoryUnitOfWorkBehaviourTemplate.TemplateId);
        }

        public static string GetStateStoreUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateStoreUnitOfWorkTemplate.TemplateId);
        }

        public static string GetStateStoreUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateStoreUnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}