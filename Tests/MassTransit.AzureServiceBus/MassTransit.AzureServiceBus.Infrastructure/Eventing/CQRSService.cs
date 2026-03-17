using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.IntegrationServices;
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