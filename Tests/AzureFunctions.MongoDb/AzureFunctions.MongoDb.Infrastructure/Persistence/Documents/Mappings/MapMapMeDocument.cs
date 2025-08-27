using System;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Mappings
{
    internal class MapMapMeDocument : IMapMapMeDocument, IMongoDbDocument<MapMapMe, MapMapMeDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public MapMapMe ToEntity(MapMapMe? entity = default)
        {
            entity ??= new MapMapMe();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public MapMapMeDocument PopulateFromEntity(MapMapMe entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static MapMapMeDocument? FromEntity(MapMapMe? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapMapMeDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapMapMeDocument> GetIdFilter(string id)
        {
            return Builders<MapMapMeDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapMapMeDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapMapMeDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapMapMeDocument>.Filter.In(d => d.Id, ids);
        }
    }
}