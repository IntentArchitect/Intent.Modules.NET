using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Associations
{
    internal class F_OptionalDependentDocument : IF_OptionalDependentDocument, IMongoDbDocument<F_OptionalDependent, F_OptionalDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string? FOptionalAggregateNavId { get; set; }

        public F_OptionalDependent ToEntity(F_OptionalDependent? entity = default)
        {
            entity ??= new F_OptionalDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.FOptionalAggregateNavId = FOptionalAggregateNavId;

            return entity;
        }

        public F_OptionalDependentDocument PopulateFromEntity(F_OptionalDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            FOptionalAggregateNavId = entity.FOptionalAggregateNavId;

            return this;
        }

        public static F_OptionalDependentDocument? FromEntity(F_OptionalDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new F_OptionalDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<F_OptionalDependentDocument> GetIdFilter(string id)
        {
            return Builders<F_OptionalDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<F_OptionalDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<F_OptionalDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<F_OptionalDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<F_OptionalDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<F_OptionalDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}