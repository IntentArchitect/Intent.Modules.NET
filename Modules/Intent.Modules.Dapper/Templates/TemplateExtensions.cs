using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapper.Templates.Repository;
using Intent.Modules.Dapper.Templates.RepositoryBase;
using Intent.Modules.Dapper.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates
{
    public static class TemplateExtensions
    {
        public static string GetRepositoryName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetRepositoryName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(RepositoryTemplate.TemplateId, model);
        }

        public static string GetRepositoryBaseName(this IIntentTemplate template)
        {
            return template.GetTypeName(RepositoryBaseTemplate.TemplateId);
        }

        public static string GetRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        }

    }
}