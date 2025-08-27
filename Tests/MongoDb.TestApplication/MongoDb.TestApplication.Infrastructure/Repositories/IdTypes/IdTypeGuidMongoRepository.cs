using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Documents.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.IdTypes
{
    internal class IdTypeGuidMongoRepository : MongoRepositoryBase<IdTypeGuid, IdTypeGuidDocument, IIdTypeGuidDocument, Guid>, IIdTypeGuidRepository
    {
        public IdTypeGuidMongoRepository(IMongoCollection<IdTypeGuidDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}