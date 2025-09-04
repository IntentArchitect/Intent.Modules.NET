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
    internal class B_OptionalDependentDocument : IB_OptionalDependentDocument, IMongoDbDocument<B_OptionalDependent, B_OptionalDependentDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public B_OptionalDependent ToEntity(B_OptionalDependent? entity = default)
        {
            entity ??= new B_OptionalDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public B_OptionalDependentDocument PopulateFromEntity(B_OptionalDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static B_OptionalDependentDocument? FromEntity(B_OptionalDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new B_OptionalDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<B_OptionalDependentDocument> GetIdFilter(string id)
        {
            return Builders<B_OptionalDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<B_OptionalDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<B_OptionalDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<B_OptionalDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<B_OptionalDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<B_OptionalDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}