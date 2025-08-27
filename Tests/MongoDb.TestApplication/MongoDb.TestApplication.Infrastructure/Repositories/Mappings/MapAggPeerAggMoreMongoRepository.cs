using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Mappings;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Mappings
{
    internal class MapAggPeerAggMoreMongoRepository : MongoRepositoryBase<MapAggPeerAggMore, MapAggPeerAggMoreDocument, IMapAggPeerAggMoreDocument, string>, IMapAggPeerAggMoreRepository
    {
        public MapAggPeerAggMoreMongoRepository(IMongoCollection<MapAggPeerAggMoreDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}