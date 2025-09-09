using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Documents;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories
{
    internal class DerivedMongoRepository : MongoRepositoryBase<Derived, DerivedDocument, IDerivedDocument, string>, IDerivedRepository
    {
        public DerivedMongoRepository(IMongoCollection<DerivedDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}