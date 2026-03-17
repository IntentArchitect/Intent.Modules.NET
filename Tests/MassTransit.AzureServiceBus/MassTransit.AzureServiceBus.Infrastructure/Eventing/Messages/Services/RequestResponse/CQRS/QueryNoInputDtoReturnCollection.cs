using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.RequestResponse.CQRS
{
    public class QueryNoInputDtoReturnCollection : IMapperRequest
    {
        public QueryNoInputDtoReturnCollection()
        {
        }

        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection.QueryNoInputDtoReturnCollection();
        }
    }
}