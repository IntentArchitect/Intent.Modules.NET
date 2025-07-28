using DynamoDbTests.EnumAsStrings.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepositoryInterface", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Repositories
{
    public interface IDynamoDBRepository<TDomain, in TPartitionKey> : IRepository<TDomain>
    {
        IDynamoDBUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindByKeyAsync(TPartitionKey partitionKey, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByKeysAsync(IEnumerable<TPartitionKey> partitionKeys, CancellationToken cancellationToken = default);
        Task<TDomain?> FindByIdAsync(TPartitionKey id, CancellationToken cancellationToken = default) => FindByKeyAsync(id, cancellationToken);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<TPartitionKey> ids, CancellationToken cancellationToken = default) => FindByKeysAsync(ids, cancellationToken);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    }

    public interface IDynamoDBRepository<TDomain, TPartitionKey, TSortKey> : IRepository<TDomain>
    {
        IDynamoDBUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindByKeyAsync(TPartitionKey partitionKey, TSortKey sortKey, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByKeysAsync(IEnumerable<(TPartitionKey Partition, TSortKey Sort)> keys, CancellationToken cancellationToken = default);
        Task<TDomain?> FindByIdAsync((TPartitionKey Partition, TSortKey Sort) id, CancellationToken cancellationToken = default) => FindByKeyAsync(id.Partition, id.Sort, cancellationToken);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<(TPartitionKey Partition, TSortKey Sort)> ids, CancellationToken cancellationToken = default) => FindByKeysAsync(ids, cancellationToken);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    }
}