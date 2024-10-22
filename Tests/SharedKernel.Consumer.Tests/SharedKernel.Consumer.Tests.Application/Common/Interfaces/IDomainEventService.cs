using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Common.Interfaces
{
    public interface IDomainEventService : SharedKernel.Kernel.Tests.Application.Common.Interfaces.IDomainEventService
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}