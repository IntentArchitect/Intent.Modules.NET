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
    [BsonDiscriminator(nameof(MapImplyOptional), Required = true)]
    internal class MapImplyOptionalDocument : IMapImplyOptionalDocument, IMongoDbDocument<MapImplyOptional, MapImplyOptionalDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Description { get; set; }

        public MapImplyOptional ToEntity(MapImplyOptional? entity = default)
        {
            entity ??= new MapImplyOptional();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");

            return entity;
        }

        public MapImplyOptionalDocument PopulateFromEntity(MapImplyOptional entity)
        {
            Id = entity.Id;
            Description = entity.Description;

            return this;
        }

        public static MapImplyOptionalDocument? FromEntity(MapImplyOptional? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapImplyOptionalDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapImplyOptionalDocument> GetIdFilter(string id)
        {
            return Builders<MapImplyOptionalDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapImplyOptionalDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapImplyOptionalDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapImplyOptionalDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapImplyOptionalDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapImplyOptionalDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}