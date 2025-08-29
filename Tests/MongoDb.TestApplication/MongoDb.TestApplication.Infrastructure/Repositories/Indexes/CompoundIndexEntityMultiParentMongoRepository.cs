using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Indexes
{
    internal class CompoundIndexEntityMultiParentMongoRepository : MongoRepositoryBase<CompoundIndexEntityMultiParent, CompoundIndexEntityMultiParentDocument, ICompoundIndexEntityMultiParentDocument, string>, ICompoundIndexEntityMultiParentRepository
    {
        public CompoundIndexEntityMultiParentMongoRepository(IMongoCollection<CompoundIndexEntityMultiParentDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}