using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Services.RequestResponse.CQRS;
using MassTransit.RequestResponse.Client.Application.IntegrationServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientImplementations.ServiceRequestClient", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Infrastructure.Eventing
{
    public class RabbitMqCQRSService : IRabbitMqCQRSService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqCQRSService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandResponseDto> CommandDtoReturnAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandDtoReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandDtoReturn>>();
            var mappedCommand = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandDtoReturn(command);
            var response = await client.GetResponse<RequestCompletedMessage<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandResponseDto>>(mappedCommand, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.ToDto();
            return mappedResponse;
        }

        public async Task<Guid> CommandGuidReturnAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandGuidReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandGuidReturn>>();
            var mappedCommand = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandGuidReturn(command);
            var response = await client.GetResponse<RequestCompletedMessage<Guid>>(mappedCommand, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload;
            return mappedResponse;
        }

        public async Task CommandNoParamAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandNoParam command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandNoParam>>();
            var mappedCommand = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandNoParam(command);
            await client.GetResponse<RequestCompletedMessage>(mappedCommand, ConfigureClient, cancellationToken);
        }

        public async Task CommandVoidReturnAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandVoidReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandVoidReturn>>();
            var mappedCommand = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandVoidReturn(command);
            await client.GetResponse<RequestCompletedMessage>(mappedCommand, ConfigureClient, cancellationToken);
        }

        public async Task<Guid> QueryGuidReturnAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.QueryGuidReturn query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryGuidReturn>>();
            var mappedQuery = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryGuidReturn(query);
            var response = await client.GetResponse<RequestCompletedMessage<Guid>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload;
            return mappedResponse;
        }

        public async Task<List<MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDto>> QueryNoInputDtoReturnCollectionAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>>();
            var mappedQuery = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection(query);
            var response = await client.GetResponse<RequestCompletedMessage<List<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDto>>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.Select(x => x.ToDto()).ToList();
            return mappedResponse;
        }

        public async Task<MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDto> QueryResponseDtoReturnAsync(
            MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDtoReturn query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDtoReturn>>();
            var mappedQuery = new MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDtoReturn(query);
            var response = await client.GetResponse<RequestCompletedMessage<MassTransit.RabbitMQ.Services.RequestResponse.CQRS.QueryResponseDto>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.ToDto();
            return mappedResponse;
        }

        public void Dispose()
        {
        }

        private static void ConfigureClient<T>(IRequestPipeConfigurator<T> config)
            where T : class
        {
            config.UseRetry(retry => retry.None());
        }
    }
}