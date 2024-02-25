using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Configuration;
using MassTransit.Middleware;
using MassTransit.Observables;
using MassTransit.RabbitMQ.Application.IntegrationServices;
using MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.RabbitMQ.Services.RequestResponse.CQRS;
using MassTransit.RetryPolicies;
using MassTransit.RetryPolicies.ExceptionFilters;
using Microsoft.Extensions.DependencyInjection;
using CommandDtoReturn = MassTransit.RabbitMQ.Services.RequestResponse.CQRS.CommandDtoReturn;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientImplementations.ServiceRequestClient", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure.Eventing
{
    public class CQRSService : ICQRSService
    {
        private readonly IServiceProvider _serviceProvider;

        public CQRSService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandResponseDto> CommandDtoReturnAsync(
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandDtoReturn command,
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
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandGuidReturn command,
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
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandNoParam command,
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
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandVoidReturn command,
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
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryGuidReturn query,
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

        public async Task<List<MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto>> QueryNoInputDtoReturnCollectionAsync(
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection query,
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

        public async Task<MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto> QueryResponseDtoReturnAsync(
            MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDtoReturn query,
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

        private static void ConfigureClient<T>(IRequestPipeConfigurator<T> config)
            where T : class
        {
            config.UseRetry(retry => retry.None());
        }

        public void Dispose()
        {
        }
    }
}