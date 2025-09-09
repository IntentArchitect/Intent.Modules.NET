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
    internal class B_OptionalDependentMongoRepository : MongoRepositoryBase<B_OptionalDependent, B_OptionalDependentDocument, IB_OptionalDependentDocument, string>, IB_OptionalDependentRepository
    {
        public B_OptionalDependentMongoRepository(IMongoCollection<B_OptionalDependentDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}