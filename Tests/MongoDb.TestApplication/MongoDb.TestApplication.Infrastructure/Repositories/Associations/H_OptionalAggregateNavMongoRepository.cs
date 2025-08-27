using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Associations
{
    internal class H_OptionalAggregateNavMongoRepository : MongoRepositoryBase<H_OptionalAggregateNav, H_OptionalAggregateNavDocument, IH_OptionalAggregateNavDocument, string>, IH_OptionalAggregateNavRepository
    {
        public H_OptionalAggregateNavMongoRepository(IMongoCollection<H_OptionalAggregateNavDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}