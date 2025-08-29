using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories.IdTypes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.IdTypes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.IdTypes
{
    internal class IdTypeGuidMongoRepository : MongoRepositoryBase<IdTypeGuid, IdTypeGuidDocument, IIdTypeGuidDocument, Guid>, IIdTypeGuidRepository
    {
        public IdTypeGuidMongoRepository(IMongoCollection<IdTypeGuidDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}