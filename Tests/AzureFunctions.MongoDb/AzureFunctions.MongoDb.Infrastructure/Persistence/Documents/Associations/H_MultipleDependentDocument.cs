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
    [BsonDiscriminator(nameof(H_MultipleDependent), Required = true)]
    internal class H_MultipleDependentDocument : IH_MultipleDependentDocument, IMongoDbDocument<H_MultipleDependent, H_MultipleDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string? HOptionalAggregateNavId { get; set; }

        public H_MultipleDependent ToEntity(H_MultipleDependent? entity = default)
        {
            entity ??= new H_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.HOptionalAggregateNavId = HOptionalAggregateNavId;

            return entity;
        }

        public H_MultipleDependentDocument PopulateFromEntity(H_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            HOptionalAggregateNavId = entity.HOptionalAggregateNavId;

            return this;
        }

        public static H_MultipleDependentDocument? FromEntity(H_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new H_MultipleDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<H_MultipleDependentDocument> GetIdFilter(string id)
        {
            return Builders<H_MultipleDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<H_MultipleDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<H_MultipleDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<H_MultipleDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<H_MultipleDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<H_MultipleDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}