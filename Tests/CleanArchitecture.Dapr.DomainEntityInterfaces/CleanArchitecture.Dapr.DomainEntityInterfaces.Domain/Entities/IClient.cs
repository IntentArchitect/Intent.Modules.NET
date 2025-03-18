using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Entities
{
    public interface IClient
    {
        string Id { get; set; }

        string Name { get; set; }
    }
}