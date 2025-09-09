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
    internal class MapAggPeerDocument : IMapAggPeerDocument, IMongoDbDocument<MapAggPeer, MapAggPeerDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string PeerAtt { get; set; }
        public string MapAggPeerAggId { get; set; }
        public string MapMapMeId { get; set; }
        public IMapPeerCompChildDocument MapPeerCompChild { get; set; }

        public MapAggPeer ToEntity(MapAggPeer? entity = default)
        {
            entity ??= new MapAggPeer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PeerAtt = PeerAtt ?? throw new Exception($"{nameof(entity.PeerAtt)} is null");
            entity.MapAggPeerAggId = MapAggPeerAggId ?? throw new Exception($"{nameof(entity.MapAggPeerAggId)} is null");
            entity.MapMapMeId = MapMapMeId ?? throw new Exception($"{nameof(entity.MapMapMeId)} is null");
            entity.MapPeerCompChild = (MapPeerCompChild as MapPeerCompChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.MapPeerCompChild)} is null");

            return entity;
        }

        public MapAggPeerDocument PopulateFromEntity(MapAggPeer entity)
        {
            Id = entity.Id;
            PeerAtt = entity.PeerAtt;
            MapAggPeerAggId = entity.MapAggPeerAggId;
            MapMapMeId = entity.MapMapMeId;
            MapPeerCompChild = MapPeerCompChildDocument.FromEntity(entity.MapPeerCompChild)!;

            return this;
        }

        public static MapAggPeerDocument? FromEntity(MapAggPeer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapAggPeerDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapAggPeerDocument> GetIdFilter(string id)
        {
            return Builders<MapAggPeerDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapAggPeerDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapAggPeerDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapAggPeerDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapAggPeerDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapAggPeerDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}