using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepository;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.DataContractExtensionMethods;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.EFRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCustomRepositoryName<T>(this IIntentTemplate<T> template) where T : RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryName(this IIntentTemplate template, RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryTemplate.TemplateId, model);
        }

        public static string GetCustomRepositoryInterfaceName<T>(this IIntentTemplate<T> template) where T : RepositoryModel
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetCustomRepositoryInterfaceName(this IIntentTemplate template, RepositoryModel model)
        {
            return template.GetTypeName(CustomRepositoryInterfaceTemplate.TemplateId, model);
        }

        public static string GetDataContractExtensionMethodsName<T>(this IIntentTemplate<T> template) where T : DataContractModel
        {
            return template.GetTypeName(DataContractExtensionMethodsTemplate.TemplateId, template.Model);
        }

        public static string GetDataContractExtensionMethodsName(this IIntentTemplate template, DataContractModel model)
        {
            return template.GetTypeName(DataContractExtensionMethodsTemplate.TemplateId, model);
        }

        public static string GetEFRepositoryInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EFRepositoryInterfaceTemplate.TemplateId);
        }

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

    }
}