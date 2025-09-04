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
    [BsonDiscriminator(nameof(MapPeerCompChildAgg), Required = true)]
    internal class MapPeerCompChildAggDocument : IMapPeerCompChildAggDocument, IMongoDbDocument<MapPeerCompChildAgg, MapPeerCompChildAggDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string MapPeerCompChildAggAtt { get; set; }

        public MapPeerCompChildAgg ToEntity(MapPeerCompChildAgg? entity = default)
        {
            entity ??= new MapPeerCompChildAgg();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MapPeerCompChildAggAtt = MapPeerCompChildAggAtt ?? throw new Exception($"{nameof(entity.MapPeerCompChildAggAtt)} is null");

            return entity;
        }

        public MapPeerCompChildAggDocument PopulateFromEntity(MapPeerCompChildAgg entity)
        {
            Id = entity.Id;
            MapPeerCompChildAggAtt = entity.MapPeerCompChildAggAtt;

            return this;
        }

        public static MapPeerCompChildAggDocument? FromEntity(MapPeerCompChildAgg? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapPeerCompChildAggDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapPeerCompChildAggDocument> GetIdFilter(string id)
        {
            return Builders<MapPeerCompChildAggDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapPeerCompChildAggDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapPeerCompChildAggDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapPeerCompChildAggDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapPeerCompChildAggDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapPeerCompChildAggDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}