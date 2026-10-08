using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Services.Guardrails.Contracts.Templates.CommandContract;
using Intent.Modules.Services.Guardrails.Contracts.Templates.DomainEventHandlerContract;
using Intent.Modules.Services.Guardrails.Contracts.Templates.DtoContract;
using Intent.Modules.Services.Guardrails.Contracts.Templates.EnumContract;
using Intent.Modules.Services.Guardrails.Contracts.Templates.QueryContract;
using Intent.Modules.Services.Guardrails.Contracts.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCommandContractName<T>(this IIntentTemplate<T> template) where T : CommandModel
        {
            return template.GetTypeName(CommandContractTemplate.TemplateId, template.Model);
        }

        public static string GetCommandContractName(this IIntentTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandContractTemplate.TemplateId, model);
        }

        public static string GetDomainEventHandlerContractName<T>(this IIntentTemplate<T> template) where T : IElement
        {
            return template.GetTypeName(DomainEventHandlerContractTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventHandlerContractName(this IIntentTemplate template, IElement model)
        {
            return template.GetTypeName(DomainEventHandlerContractTemplate.TemplateId, model);
        }

        public static string GetDtoContractName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, template.Model);
        }

        public static string GetDtoContractName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, model);
        }

        public static string GetEnumContractName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, template.Model);
        }

        public static string GetEnumContractName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, model);
        }

        public static string GetQueryContractName<T>(this IIntentTemplate<T> template) where T : QueryModel
        {
            return template.GetTypeName(QueryContractTemplate.TemplateId, template.Model);
        }

        public static string GetQueryContractName(this IIntentTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryContractTemplate.TemplateId, model);
        }

        public static string GetServiceContractName<T>(this IIntentTemplate<T> template) where T : ServiceModel
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetServiceContractName(this IIntentTemplate template, ServiceModel model)
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, model);
        }

    }
}