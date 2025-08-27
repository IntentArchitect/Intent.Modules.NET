using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes
{
    internal class MultikeyIndexEntityDocument : IMultikeyIndexEntityDocument, IMongoDbDocument<MultikeyIndexEntity, MultikeyIndexEntityDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public IEnumerable<string> MultiKey { get; set; }
        public string SomeField { get; set; }

        public MultikeyIndexEntity ToEntity(MultikeyIndexEntity? entity = default)
        {
            entity ??= new MultikeyIndexEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MultiKey = MultiKey.ToList() ?? throw new Exception($"{nameof(entity.MultiKey)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");

            return entity;
        }

        public MultikeyIndexEntityDocument PopulateFromEntity(MultikeyIndexEntity entity)
        {
            Id = entity.Id;
            MultiKey = entity.MultiKey.ToList();
            SomeField = entity.SomeField;

            return this;
        }

        public static MultikeyIndexEntityDocument? FromEntity(MultikeyIndexEntity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MultikeyIndexEntityDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MultikeyIndexEntityDocument> GetIdFilter(string id)
        {
            return Builders<MultikeyIndexEntityDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MultikeyIndexEntityDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MultikeyIndexEntityDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MultikeyIndexEntityDocument>.Filter.In(d => d.Id, ids);
        }
    }
}