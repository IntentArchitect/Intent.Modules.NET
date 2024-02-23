using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransitFinbuckle.Test.Application.IntegrationServices;
using MassTransitFinbuckle.Test.Services.RequestResponse;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.ClientImplementation.ServiceRequestClient", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure.Eventing
{
    public class RequestResponseService : IRequestResponseService
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestResponseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task TestAsync(
            MassTransitFinbuckle.Test.Application.IntegrationServices.Contracts.Services.RequestResponse.TestCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var client = _serviceProvider.GetRequiredService<IRequestClient<MassTransitFinbuckle.Test.Services.RequestResponse.TestCommand>>();
            var mappedCommand = new MassTransitFinbuckle.Test.Services.RequestResponse.TestCommand(command);
            await client.GetResponse<RequestCompletedMessage>(mappedCommand, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}