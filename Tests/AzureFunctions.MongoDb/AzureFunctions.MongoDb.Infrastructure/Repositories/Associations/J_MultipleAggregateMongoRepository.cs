using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories;
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
    internal class J_MultipleAggregateMongoRepository : MongoRepositoryBase<J_MultipleAggregate, J_MultipleAggregateDocument, IJ_MultipleAggregateDocument, string>, IJ_MultipleAggregateRepository
    {
        public J_MultipleAggregateMongoRepository(IMongoCollection<J_MultipleAggregateDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}