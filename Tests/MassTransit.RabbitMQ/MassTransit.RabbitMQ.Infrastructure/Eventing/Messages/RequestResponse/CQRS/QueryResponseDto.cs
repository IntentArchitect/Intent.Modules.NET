using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperResponseMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.RequestResponse.CQRS
{
    public class QueryResponseDto
    {
        public QueryResponseDto()
        {
        }

        public QueryResponseDto(MassTransit.RabbitMQ.Application.RequestResponse.CQRS.QueryResponseDto dto)
        {
            Result = dto.Result;
        }
        public string Result { get; set; }

        public MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto ToDto()
        {
            return MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto.Create(Result);
        }
    }
}