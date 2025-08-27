using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings
{
    internal class MapAggPeerAggMoreDocument : IMapAggPeerAggMoreDocument, IMongoDbDocument<MapAggPeerAggMore, MapAggPeerAggMoreDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
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
    }
}