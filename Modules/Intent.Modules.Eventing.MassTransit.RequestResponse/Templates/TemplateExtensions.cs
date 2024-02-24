using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.EnumContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.PagedResult;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.ServiceContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientImplementations.ServiceRequestClient;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.Consumers.MediatRConsumer;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestInterface;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.RequestCompletedMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.ResponseMappingFactory;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates
{
    public static class TemplateExtensions
    {
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

        public static string GetPagedResultName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedResultTemplate.TemplateId);
        }

        public static string GetServiceContractName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetServiceContractName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, model);
        }

        public static string GetServiceRequestClientName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(ServiceRequestClientTemplate.TemplateId, template.Model);
        }

        public static string GetServiceRequestClientName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceRequestClientTemplate.TemplateId, model);
        }

        public static string GetMediatRConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(MediatRConsumerTemplate.TemplateId);
        }

        public static string GetMapperRequestInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MapperRequestInterfaceTemplate.TemplateId);
        }

        public static string GetMapperRequestMessageName<T>(this IIntentTemplate<T> template) where T : HybridDtoModel
        {
            return template.GetTypeName(MapperRequestMessageTemplate.TemplateId, template.Model);
        }

        public static string GetMapperRequestMessageName(this IIntentTemplate template, HybridDtoModel model)
        {
            return template.GetTypeName(MapperRequestMessageTemplate.TemplateId, model);
        }

        public static string GetMapperResponseMessageName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(MapperResponseMessageTemplate.TemplateId, template.Model);
        }

        public static string GetMapperResponseMessageName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(MapperResponseMessageTemplate.TemplateId, model);
        }

        public static string GetRequestCompletedMessageName(this IIntentTemplate template)
        {
            return template.GetTypeName(RequestCompletedMessageTemplate.TemplateId);
        }

        public static string GetResponseMappingFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(ResponseMappingFactoryTemplate.TemplateId);
        }

    }
}