using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Modules.MongoDb.Templates.Integration.UnitOfWorkBehaviour;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApplicationMongoDbContextName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ApplicationMongoDbContextTemplate.TemplateId);
        }

        public static string GetMongoDbUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetUnitOfWorkBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(UnitOfWorkBehaviourTemplate.TemplateId);
        }

    }
}