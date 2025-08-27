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
    internal class G_RequiredCompositeNavMongoRepository : MongoRepositoryBase<G_RequiredCompositeNav, G_RequiredCompositeNavDocument, IG_RequiredCompositeNavDocument, string>, IG_RequiredCompositeNavRepository
    {
        public G_RequiredCompositeNavMongoRepository(IMongoCollection<G_RequiredCompositeNavDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}