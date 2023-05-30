using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApplicationMongoDbContextName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApplicationMongoDbContextTemplate.TemplateId);
        }

        public static string GetMongoDbUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}