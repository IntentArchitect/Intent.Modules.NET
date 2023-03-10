using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Modules.MongoDb.Templates.BsonClassMap;
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

        public static string GetBsonClassMapName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(BsonClassMapTemplate.TemplateId, template.Model);
        }

        public static string GetBsonClassMapName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(BsonClassMapTemplate.TemplateId, model);
        }

        public static string GetMongoDbUnitOfWorkInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId);
        }

    }
}