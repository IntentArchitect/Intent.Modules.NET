using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableEntityInterface", Version = "1.0")]

namespace TableStorage.Tests.Domain.Repositories.TableEntities
{
    public interface IInvoiceTableEntity
    {
        string PartitionKey { get; }
        string RowKey { get; }
        DateTime IssuedData { get; }
        string OrderPartitionKey { get; }
        string OrderRowKey { get; }
    }
}