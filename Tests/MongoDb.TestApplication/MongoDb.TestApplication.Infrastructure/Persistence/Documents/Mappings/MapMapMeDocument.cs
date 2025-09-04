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
    internal class MapMapMeDocument : IMapMapMeDocument, IMongoDbDocument<MapMapMe, MapMapMeDocument, string>
    {
        [BsonId]
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

        public static Expression<Func<MapMapMeDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapMapMeDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}