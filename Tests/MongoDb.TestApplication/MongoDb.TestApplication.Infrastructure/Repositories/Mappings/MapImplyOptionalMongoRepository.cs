using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Mappings;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Mappings
{
    internal class MapImplyOptionalMongoRepository : MongoRepositoryBase<MapImplyOptional, MapImplyOptionalDocument, IMapImplyOptionalDocument, string>, IMapImplyOptionalRepository
    {
        public MapImplyOptionalMongoRepository(IMongoCollection<MapImplyOptionalDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}