using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.DomainEvents
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}