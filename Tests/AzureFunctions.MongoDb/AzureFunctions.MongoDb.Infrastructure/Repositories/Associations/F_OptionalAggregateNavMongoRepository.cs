using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Associations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Associations
{
    internal class F_OptionalAggregateNavMongoRepository : MongoRepositoryBase<F_OptionalAggregateNav, F_OptionalAggregateNavDocument, IF_OptionalAggregateNavDocument, string>, IF_OptionalAggregateNavRepository
    {
        public F_OptionalAggregateNavMongoRepository(IMongoCollection<F_OptionalAggregateNavDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}