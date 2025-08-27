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
    internal class A_RequiredCompositeMongoRepository : MongoRepositoryBase<A_RequiredComposite, A_RequiredCompositeDocument, IA_RequiredCompositeDocument, string>, IA_RequiredCompositeRepository
    {
        public A_RequiredCompositeMongoRepository(IMongoCollection<A_RequiredCompositeDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}