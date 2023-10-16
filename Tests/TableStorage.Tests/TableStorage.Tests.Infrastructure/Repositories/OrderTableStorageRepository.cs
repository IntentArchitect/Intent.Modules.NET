using Azure.Data.Tables;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;
using TableStorage.Tests.Domain.Repositories;
using TableStorage.Tests.Infrastructure.Persistence;
using TableStorage.Tests.Infrastructure.Persistence.Tables;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageRepository", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Repositories
{
    internal class OrderTableStorageRepository : TableStorageRepositoryBase<Order, Order, OrderTableEntity>, IOrderRepository
    {
        public OrderTableStorageRepository(TableStorageUnitOfWork unitOfWork,
            TableServiceClient tableServiceClient,
            string tableName = nameof(Order)) : base(unitOfWork, tableServiceClient, tableName)
        {
        }
    }
}