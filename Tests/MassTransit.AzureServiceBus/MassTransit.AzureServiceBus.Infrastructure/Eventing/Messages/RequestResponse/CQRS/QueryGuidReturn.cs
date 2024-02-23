using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryGuidReturn;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.RequestResponse.CQRS
{
    public class QueryGuidReturn : IMapperRequest
    {
        public QueryGuidReturn()
        {
        }

        public QueryGuidReturn(Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryGuidReturn dto)
        {
            Input = dto.Input;
        }

        public string Input { get; set; }

        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.QueryGuidReturn.QueryGuidReturn(Input);
        }
    }
}