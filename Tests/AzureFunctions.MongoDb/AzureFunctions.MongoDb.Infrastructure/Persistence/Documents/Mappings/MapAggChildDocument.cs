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
    internal class MapAggChildDocument : IMapAggChildDocument, IMongoDbDocument<MapAggChild, MapAggChildDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string ChildName { get; set; }

        public MapAggChild ToEntity(MapAggChild? entity = default)
        {
            entity ??= new MapAggChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ChildName = ChildName ?? throw new Exception($"{nameof(entity.ChildName)} is null");

            return entity;
        }

        public MapAggChildDocument PopulateFromEntity(MapAggChild entity)
        {
            Id = entity.Id;
            ChildName = entity.ChildName;

            return this;
        }

        public static MapAggChildDocument? FromEntity(MapAggChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapAggChildDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapAggChildDocument> GetIdFilter(string id)
        {
            return Builders<MapAggChildDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapAggChildDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapAggChildDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapAggChildDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapAggChildDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapAggChildDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}