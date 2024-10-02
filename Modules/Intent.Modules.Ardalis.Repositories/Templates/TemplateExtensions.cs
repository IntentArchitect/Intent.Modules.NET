using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Ardalis.Repositories.Templates.ReadRepositoryInterface;
using Intent.Modules.Ardalis.Repositories.Templates.Specification;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories.Templates
{
    public static class TemplateExtensions
    {
        public static string GetReadRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(ReadRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetReadRepositoryInterfaceName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(ReadRepositoryInterfaceTemplate.TemplateId, model);
        }

        public static string GetSpecificationName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(SpecificationTemplate.TemplateId, template.Model);
        }

        public static string GetSpecificationName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(SpecificationTemplate.TemplateId, model);
        }

    }
}