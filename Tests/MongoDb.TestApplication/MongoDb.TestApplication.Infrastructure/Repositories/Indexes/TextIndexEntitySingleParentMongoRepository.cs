using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Indexes;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Indexes
{
    internal class TextIndexEntitySingleParentMongoRepository : MongoRepositoryBase<TextIndexEntitySingleParent, TextIndexEntitySingleParentDocument, ITextIndexEntitySingleParentDocument, string>, ITextIndexEntitySingleParentRepository
    {
        public TextIndexEntitySingleParentMongoRepository(IMongoCollection<TextIndexEntitySingleParentDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}