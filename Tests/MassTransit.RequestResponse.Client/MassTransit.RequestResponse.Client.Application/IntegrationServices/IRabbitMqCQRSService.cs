using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.ServiceContract", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.IntegrationServices
{
    public interface IRabbitMqCQRSService : IDisposable
    {
        Task<CommandResponseDto> CommandDtoReturnAsync(CommandDtoReturn command, CancellationToken cancellationToken = default);
        Task<Guid> CommandGuidReturnAsync(CommandGuidReturn command, CancellationToken cancellationToken = default);
        Task CommandNoParamAsync(CommandNoParam command, CancellationToken cancellationToken = default);
        Task CommandVoidReturnAsync(CommandVoidReturn command, CancellationToken cancellationToken = default);
        Task<Guid> QueryGuidReturnAsync(QueryGuidReturn query, CancellationToken cancellationToken = default);
        Task<List<QueryResponseDto>> QueryNoInputDtoReturnCollectionAsync(QueryNoInputDtoReturnCollection query, CancellationToken cancellationToken = default);
        Task<QueryResponseDto> QueryResponseDtoReturnAsync(QueryResponseDtoReturn query, CancellationToken cancellationToken = default);
    }
}