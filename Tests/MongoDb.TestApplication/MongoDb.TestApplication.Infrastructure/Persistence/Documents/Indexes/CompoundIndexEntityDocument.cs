using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes
{
    internal class CompoundIndexEntityDocument : ICompoundIndexEntityDocument, IMongoDbDocument<CompoundIndexEntity, CompoundIndexEntityDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public string CompoundOne { get; set; }
        public string CompoundTwo { get; set; }

        public CompoundIndexEntity ToEntity(CompoundIndexEntity? entity = default)
        {
            entity ??= new CompoundIndexEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.CompoundOne = CompoundOne ?? throw new Exception($"{nameof(entity.CompoundOne)} is null");
            entity.CompoundTwo = CompoundTwo ?? throw new Exception($"{nameof(entity.CompoundTwo)} is null");

            return entity;
        }

        public CompoundIndexEntityDocument PopulateFromEntity(CompoundIndexEntity entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            CompoundOne = entity.CompoundOne;
            CompoundTwo = entity.CompoundTwo;

            return this;
        }

        public static CompoundIndexEntityDocument? FromEntity(CompoundIndexEntity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompoundIndexEntityDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CompoundIndexEntityDocument> GetIdFilter(string id)
        {
            return Builders<CompoundIndexEntityDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CompoundIndexEntityDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CompoundIndexEntityDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CompoundIndexEntityDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<CompoundIndexEntityDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CompoundIndexEntityDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}