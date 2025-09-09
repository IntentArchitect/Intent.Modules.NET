using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Mappings;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Mappings
{
    internal class MapMapMeMongoRepository : MongoRepositoryBase<MapMapMe, MapMapMeDocument, IMapMapMeDocument, string>, IMapMapMeRepository
    {
        public MapMapMeMongoRepository(IMongoCollection<MapMapMeDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}