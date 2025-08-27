using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Repositories
{
    internal class OrderMongoRepository : MongoRepositoryBase<Order, OrderDocument, IOrderDocument, string>, IOrderRepository
    {
        public OrderMongoRepository(IMongoCollection<OrderDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}