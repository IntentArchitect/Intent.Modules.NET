using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Finbuckle.MultiTenant.Templates.MongoDbConnectionFactory;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.MultiTenant.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMongoDbConnectionFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(MongoDbConnectionFactoryTemplate.TemplateId);
        }

    }
}