using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Collections;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Collections;
using MongoDb.TestApplication.Domain.Repositories.Documents.Collections;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Collections
{
    internal class CustomCollectionEntityBMongoRepository : MongoRepositoryBase<CustomCollectionEntityB, CustomCollectionEntityBDocument, ICustomCollectionEntityBDocument, string>, ICustomCollectionEntityBRepository
    {
        public CustomCollectionEntityBMongoRepository(IMongoCollection<CustomCollectionEntityBDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}