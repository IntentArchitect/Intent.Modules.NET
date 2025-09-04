using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
{
    [BsonDiscriminator(nameof(MultikeyIndexEntityMultiParent), Required = true)]
    internal class MultikeyIndexEntityMultiParentDocument : IMultikeyIndexEntityMultiParentDocument, IMongoDbDocument<MultikeyIndexEntityMultiParent, MultikeyIndexEntityMultiParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public IEnumerable<IMultikeyIndexEntityMultiChildDocument> MultikeyIndexEntityMultiChild { get; set; }

        public MultikeyIndexEntityMultiParent ToEntity(MultikeyIndexEntityMultiParent? entity = default)
        {
            entity ??= new MultikeyIndexEntityMultiParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.MultikeyIndexEntityMultiChild = MultikeyIndexEntityMultiChild.Select(x => (x as MultikeyIndexEntityMultiChildDocument).ToEntity()).ToList();

            return entity;
        }

        public MultikeyIndexEntityMultiParentDocument PopulateFromEntity(MultikeyIndexEntityMultiParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            MultikeyIndexEntityMultiChild = entity.MultikeyIndexEntityMultiChild.Select(x => MultikeyIndexEntityMultiChildDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static MultikeyIndexEntityMultiParentDocument? FromEntity(MultikeyIndexEntityMultiParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MultikeyIndexEntityMultiParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MultikeyIndexEntityMultiParentDocument> GetIdFilter(string id)
        {
            return Builders<MultikeyIndexEntityMultiParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MultikeyIndexEntityMultiParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MultikeyIndexEntityMultiParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MultikeyIndexEntityMultiParentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MultikeyIndexEntityMultiParentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MultikeyIndexEntityMultiParentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}