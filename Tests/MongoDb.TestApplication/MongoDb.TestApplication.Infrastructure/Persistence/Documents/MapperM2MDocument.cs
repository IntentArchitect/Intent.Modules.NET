using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    internal class MapperM2MDocument : IMapperM2MDocument, IMongoDbDocument<MapperM2M, MapperM2MDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Desc { get; set; }

        public MapperM2M ToEntity(MapperM2M? entity = default)
        {
            entity ??= new MapperM2M();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Desc = Desc ?? throw new Exception($"{nameof(entity.Desc)} is null");

            return entity;
        }

        public MapperM2MDocument PopulateFromEntity(MapperM2M entity)
        {
            Id = entity.Id;
            Desc = entity.Desc;

            return this;
        }

        public static MapperM2MDocument? FromEntity(MapperM2M? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapperM2MDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapperM2MDocument> GetIdFilter(string id)
        {
            return Builders<MapperM2MDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapperM2MDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapperM2MDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapperM2MDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapperM2MDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapperM2MDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}