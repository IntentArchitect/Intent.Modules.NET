using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Finbuckle.Templates.MongoDbMultiTenancyConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMongoDbMultiTenancyConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MongoDbMultiTenancyConfigurationTemplate.TemplateId);
        }

    }
}