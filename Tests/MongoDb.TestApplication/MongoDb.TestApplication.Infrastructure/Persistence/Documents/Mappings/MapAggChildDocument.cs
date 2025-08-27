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
    internal class MapAggChildDocument : IMapAggChildDocument, IMongoDbDocument<MapAggChild, MapAggChildDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
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
    }
}