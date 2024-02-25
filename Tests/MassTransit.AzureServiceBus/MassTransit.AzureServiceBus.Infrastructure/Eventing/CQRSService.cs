using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.IntegrationServices;
using MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.AzureServiceBus.Services.RequestResponse.CQRS;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientImplementations.ServiceRequestClient", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing
{
    public class CQRSService : ICQRSService
    {
        private readonly IServiceProvider _serviceProvider;

        public CQRSService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandResponseDto> CommandDtoReturnAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandDtoReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>>();
            var mappedCommand = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn(command);
            var response = await client.GetResponse<RequestCompletedMessage<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto>>(mappedCommand, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.ToDto();
            scope.Complete();
            return mappedResponse;
        }

        public async Task<Guid> CommandGuidReturnAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandGuidReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>>();
            var mappedCommand = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn(command);
            var response = await client.GetResponse<RequestCompletedMessage<Guid>>(mappedCommand, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload;
            scope.Complete();
            return mappedResponse;
        }

        public async Task CommandNoParamAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandNoParam command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>>();
            var mappedCommand = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam(command);
            await client.GetResponse<RequestCompletedMessage>(mappedCommand, ConfigureClient, cancellationToken);
            scope.Complete();
        }

        public async Task CommandVoidReturnAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.CommandVoidReturn command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>>();
            var mappedCommand = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn(command);
            await client.GetResponse<RequestCompletedMessage>(mappedCommand, ConfigureClient, cancellationToken);
            scope.Complete();
        }

        public async Task<Guid> QueryGuidReturnAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryGuidReturn query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>>();
            var mappedQuery = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn(query);
            var response = await client.GetResponse<RequestCompletedMessage<Guid>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload;
            scope.Complete();
            return mappedResponse;
        }

        public async Task<List<MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto>> QueryNoInputDtoReturnCollectionAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>>();
            var mappedQuery = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection(query);
            var response = await client.GetResponse<RequestCompletedMessage<List<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.Select(x => x.ToDto()).ToList();
            scope.Complete();
            return mappedResponse;
        }

        public async Task<MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDto> QueryResponseDtoReturnAsync(
            MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS.QueryResponseDtoReturn query,
            CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>>();
            var mappedQuery = new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn(query);
            var response = await client.GetResponse<RequestCompletedMessage<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>>(mappedQuery, ConfigureClient, cancellationToken);
            var mappedResponse = response.Message.Payload.ToDto();
            scope.Complete();
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