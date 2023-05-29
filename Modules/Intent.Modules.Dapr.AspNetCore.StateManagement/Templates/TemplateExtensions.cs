using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepository;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepositoryInterface;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepositoryUnitOfWorkBehaviour;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates
{
    public static class TemplateExtensions
    {
        public static string GetStateRepositoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateRepositoryTemplate.TemplateId);
        }

        public static string GetStateRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateRepositoryInterfaceTemplate.TemplateId);
        }

        public static string GetStateRepositoryUnitOfWorkBehaviourName(this IIntentTemplate template)
        {
            return template.GetTypeName(StateRepositoryUnitOfWorkBehaviourTemplate.TemplateId);
        }

    }
}