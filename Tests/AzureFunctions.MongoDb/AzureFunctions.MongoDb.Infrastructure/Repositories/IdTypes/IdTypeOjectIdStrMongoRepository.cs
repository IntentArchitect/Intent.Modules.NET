using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories;
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
    internal class IdTypeOjectIdStrMongoRepository : MongoRepositoryBase<IdTypeOjectIdStr, IdTypeOjectIdStrDocument, IIdTypeOjectIdStrDocument, string>, IIdTypeOjectIdStrRepository
    {
        public IdTypeOjectIdStrMongoRepository(IMongoCollection<IdTypeOjectIdStrDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}