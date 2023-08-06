using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.Publish.CleanArchDapr.TestApplication.Services.Orders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices
{
    public interface IMyService : IDisposable
    {
        Task OrderConfirmedAsync(Guid id, OrderConfirmed command, CancellationToken cancellationToken = default);
    }
}