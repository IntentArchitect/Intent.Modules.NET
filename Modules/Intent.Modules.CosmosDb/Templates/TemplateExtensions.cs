using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDb.Templates.ApplicationCosmosDbContext;
using Intent.Modules.CosmosDb.Templates.CosmosDbUnitOfWorkInterface;
using Intent.Modules.CosmosDb.Templates.Integration.UnitOfWorkBehaviour;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApplicationCosmosDbContextName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ApplicationCosmosDbContextTemplate.TemplateId);
        }

        public static string GetCosmosDbUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CosmosDbUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetUnitOfWorkBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnitOfWorkBehaviourTemplate.TemplateId);
        }

    }
}