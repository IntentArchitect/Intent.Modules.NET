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
    [BsonDiscriminator(nameof(MapCompChildAgg), Required = true)]
    internal class MapCompChildAggDocument : IMapCompChildAggDocument, IMongoDbDocument<MapCompChildAgg, MapCompChildAggDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string CompChildAggAtt { get; set; }

        public MapCompChildAgg ToEntity(MapCompChildAgg? entity = default)
        {
            entity ??= new MapCompChildAgg();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CompChildAggAtt = CompChildAggAtt ?? throw new Exception($"{nameof(entity.CompChildAggAtt)} is null");

            return entity;
        }

        public MapCompChildAggDocument PopulateFromEntity(MapCompChildAgg entity)
        {
            Id = entity.Id;
            CompChildAggAtt = entity.CompChildAggAtt;

            return this;
        }

        public static MapCompChildAggDocument? FromEntity(MapCompChildAgg? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapCompChildAggDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapCompChildAggDocument> GetIdFilter(string id)
        {
            return Builders<MapCompChildAggDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapCompChildAggDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapCompChildAggDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapCompChildAggDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapCompChildAggDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapCompChildAggDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}