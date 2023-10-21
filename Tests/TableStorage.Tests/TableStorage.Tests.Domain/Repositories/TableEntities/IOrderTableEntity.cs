using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableEntityInterface", Version = "1.0")]

namespace TableStorage.Tests.Domain.Repositories.TableEntities
{
    public interface IOrderTableEntity
    {
        string PartitionKey { get; }
        string RowKey { get; }
        string OrderNo { get; }
        decimal Amount { get; }
    }
}