using DynamoDbTests.PrivateSetters.Domain.Entities.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Repositories.Throughput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOnDemandRepository : IDynamoDBRepository<OnDemand, string>
    {
    }
}