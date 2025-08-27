using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories;
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
    internal class MapAggPeerAggMoreMongoRepository : MongoRepositoryBase<MapAggPeerAggMore, MapAggPeerAggMoreDocument, IMapAggPeerAggMoreDocument, string>, IMapAggPeerAggMoreRepository
    {
        public MapAggPeerAggMoreMongoRepository(IMongoCollection<MapAggPeerAggMoreDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}