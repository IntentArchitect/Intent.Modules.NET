using DynamoDbTests.EntityInterfaces.Domain.Entities.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Repositories.Throughput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IProvisionedRepository : IDynamoDBRepository<IProvisioned, string>
    {
    }
}