using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Indexes
{
    internal class SingleIndexEntitySingleParentMongoRepository : MongoRepositoryBase<SingleIndexEntitySingleParent, SingleIndexEntitySingleParentDocument, ISingleIndexEntitySingleParentDocument, string>, ISingleIndexEntitySingleParentRepository
    {
        public SingleIndexEntitySingleParentMongoRepository(IMongoCollection<SingleIndexEntitySingleParentDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}