using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.Documents.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.IdTypes;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.IdTypes
{
    internal class IdTypeOjectIdStrMongoRepository : MongoRepositoryBase<IdTypeOjectIdStr, IdTypeOjectIdStrDocument, IIdTypeOjectIdStrDocument, string>, IIdTypeOjectIdStrRepository
    {
        public IdTypeOjectIdStrMongoRepository(IMongoCollection<IdTypeOjectIdStrDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}