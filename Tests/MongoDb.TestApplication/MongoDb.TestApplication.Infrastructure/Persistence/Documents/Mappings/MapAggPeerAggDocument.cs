using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings
{
    [BsonDiscriminator(nameof(MapAggPeerAgg), Required = true)]
    internal class MapAggPeerAggDocument : IMapAggPeerAggDocument, IMongoDbDocument<MapAggPeerAgg, MapAggPeerAggDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string MapAggPeerAggAtt { get; set; }
        public string MapAggPeerAggMoreId { get; set; }

        public MapAggPeerAgg ToEntity(MapAggPeerAgg? entity = default)
        {
            entity ??= new MapAggPeerAgg();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MapAggPeerAggAtt = MapAggPeerAggAtt ?? throw new Exception($"{nameof(entity.MapAggPeerAggAtt)} is null");
            entity.MapAggPeerAggMoreId = MapAggPeerAggMoreId ?? throw new Exception($"{nameof(entity.MapAggPeerAggMoreId)} is null");

            return entity;
        }

        public MapAggPeerAggDocument PopulateFromEntity(MapAggPeerAgg entity)
        {
            Id = entity.Id;
            MapAggPeerAggAtt = entity.MapAggPeerAggAtt;
            MapAggPeerAggMoreId = entity.MapAggPeerAggMoreId;

            return this;
        }

        public static MapAggPeerAggDocument? FromEntity(MapAggPeerAgg? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapAggPeerAggDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapAggPeerAggDocument> GetIdFilter(string id)
        {
            return Builders<MapAggPeerAggDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapAggPeerAggDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapAggPeerAggDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapAggPeerAggDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapAggPeerAggDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapAggPeerAggDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}