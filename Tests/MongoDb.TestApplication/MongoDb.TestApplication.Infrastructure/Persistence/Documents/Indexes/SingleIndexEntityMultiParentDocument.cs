using System;
using System.Collections.Generic;
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
    internal class SingleIndexEntityMultiParentDocument : ISingleIndexEntityMultiParentDocument, IMongoDbDocument<SingleIndexEntityMultiParent, SingleIndexEntityMultiParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public IEnumerable<ISingleIndexEntityMultiChildDocument> SingleIndexEntityMultiChild { get; set; }

        public SingleIndexEntityMultiParent ToEntity(SingleIndexEntityMultiParent? entity = default)
        {
            entity ??= new SingleIndexEntityMultiParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.SingleIndexEntityMultiChild = SingleIndexEntityMultiChild.Select(x => (x as SingleIndexEntityMultiChildDocument).ToEntity()).ToList();

            return entity;
        }

        public SingleIndexEntityMultiParentDocument PopulateFromEntity(SingleIndexEntityMultiParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            SingleIndexEntityMultiChild = entity.SingleIndexEntityMultiChild.Select(x => SingleIndexEntityMultiChildDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static SingleIndexEntityMultiParentDocument? FromEntity(SingleIndexEntityMultiParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SingleIndexEntityMultiParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<SingleIndexEntityMultiParentDocument> GetIdFilter(string id)
        {
            return Builders<SingleIndexEntityMultiParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<SingleIndexEntityMultiParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<SingleIndexEntityMultiParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<SingleIndexEntityMultiParentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<SingleIndexEntityMultiParentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<SingleIndexEntityMultiParentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}