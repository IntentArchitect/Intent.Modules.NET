using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Mappings
{
    internal class MapAggPeerAggMoreDocument : IMapAggPeerAggMoreDocument, IMongoDbDocument<MapAggPeerAggMore, MapAggPeerAggMoreDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string MapAggPeerAggMoreAtt { get; set; }

        public MapAggPeerAggMore ToEntity(MapAggPeerAggMore? entity = default)
        {
            entity ??= new MapAggPeerAggMore();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MapAggPeerAggMoreAtt = MapAggPeerAggMoreAtt ?? throw new Exception($"{nameof(entity.MapAggPeerAggMoreAtt)} is null");

            return entity;
        }

        public MapAggPeerAggMoreDocument PopulateFromEntity(MapAggPeerAggMore entity)
        {
            Id = entity.Id;
            MapAggPeerAggMoreAtt = entity.MapAggPeerAggMoreAtt;

            return this;
        }

        public static MapAggPeerAggMoreDocument? FromEntity(MapAggPeerAggMore? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapAggPeerAggMoreDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapAggPeerAggMoreDocument> GetIdFilter(string id)
        {
            return Builders<MapAggPeerAggMoreDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapAggPeerAggMoreDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapAggPeerAggMoreDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapAggPeerAggMoreDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapAggPeerAggMoreDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapAggPeerAggMoreDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}