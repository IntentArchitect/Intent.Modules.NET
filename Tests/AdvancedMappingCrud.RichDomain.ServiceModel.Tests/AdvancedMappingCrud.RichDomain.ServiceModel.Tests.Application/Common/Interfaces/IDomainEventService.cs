using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}