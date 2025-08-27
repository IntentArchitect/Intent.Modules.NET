using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Collections;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Collections;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Collections
{
    internal class CustomCollectionEntityAMongoRepository : MongoRepositoryBase<CustomCollectionEntityA, CustomCollectionEntityADocument, ICustomCollectionEntityADocument, string>, ICustomCollectionEntityARepository
    {
        public CustomCollectionEntityAMongoRepository(IMongoCollection<CustomCollectionEntityADocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}