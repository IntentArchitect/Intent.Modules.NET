using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.RabbitMQ.Application.RequestResponse.CQRS.QueryResponseDtoReturn;
using MassTransit.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class QueryResponseDtoReturn : IMapperRequest
    {
        public QueryResponseDtoReturn()
        {
        }

        public QueryResponseDtoReturn(Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDtoReturn dto)
        {
            Input = dto.Input;
        }
        public string Input { get; set; }

        public object CreateRequest()
        {
            return new Application.RequestResponse.CQRS.QueryResponseDtoReturn.QueryResponseDtoReturn(Input);
        }
    }
}