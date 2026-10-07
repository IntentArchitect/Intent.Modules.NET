using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.DomainContract;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.DomainServiceContract;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.EntityContract;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.EnumContract;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.RepositoryContract;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.ValueObjectContract;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDomainContractName<T>(this IIntentTemplate<T> template) where T : DataContractModel
        {
            return template.GetTypeName(DomainContractTemplate.TemplateId, template.Model);
        }

        public static string GetDomainContractName(this IIntentTemplate template, DataContractModel model)
        {
            return template.GetTypeName(DomainContractTemplate.TemplateId, model);
        }

        public static string GetDomainServiceContractName<T>(this IIntentTemplate<T> template) where T : DomainServiceModel
        {
            return template.GetTypeName(DomainServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetDomainServiceContractName(this IIntentTemplate template, DomainServiceModel model)
        {
            return template.GetTypeName(DomainServiceContractTemplate.TemplateId, model);
        }

        public static string GetEntityContractName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(EntityContractTemplate.TemplateId, template.Model);
        }

        public static string GetEntityContractName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(EntityContractTemplate.TemplateId, model);
        }

        public static string GetEnumContractName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, template.Model);
        }

        public static string GetEnumContractName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, model);
        }

        public static string GetRepositoryContractName<T>(this IIntentTemplate<T> template) where T : RepositoryModel
        {
            return template.GetTypeName(RepositoryContractTemplate.TemplateId, template.Model);
        }

        public static string GetRepositoryContractName(this IIntentTemplate template, RepositoryModel model)
        {
            return template.GetTypeName(RepositoryContractTemplate.TemplateId, model);
        }

        public static string GetValueObjectContractName<T>(this IIntentTemplate<T> template) where T : ValueObjectModel
        {
            return template.GetTypeName(ValueObjectContractTemplate.TemplateId, template.Model);
        }

        public static string GetValueObjectContractName(this IIntentTemplate template, ValueObjectModel model)
        {
            return template.GetTypeName(ValueObjectContractTemplate.TemplateId, model);
        }

    }
}