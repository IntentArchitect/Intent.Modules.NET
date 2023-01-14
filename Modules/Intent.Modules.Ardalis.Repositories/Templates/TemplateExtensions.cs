using System.Collections.Generic;
using Intent.Modules.Ardalis.Repositories.Templates.ReadRepositoryInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories.Templates
{
    public static class TemplateExtensions
    {

        public static string GetReadRepositoryInterfaceName<T>(this IntentTemplateBase<T> template)
where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(ReadRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetReadRepositoryInterfaceName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(ReadRepositoryInterfaceTemplate.TemplateId, model);
        }

    }
}