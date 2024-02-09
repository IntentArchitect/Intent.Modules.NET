using System.Collections.Generic;
using Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates.PopulateIdsSpecimenBuilder;
using Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates.TestDataFactory;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates
{
    public static class TemplateExtensions
    {
        public static string GetPopulateIdsSpecimenBuilderName(this IIntentTemplate template)
        {
            return template.GetTypeName(PopulateIdsSpecimenBuilderTemplate.TemplateId);
        }
        public static string GetTestDataFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(TestDataFactoryTemplate.TemplateId);
        }

    }
}