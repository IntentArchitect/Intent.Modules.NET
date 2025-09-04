using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    [BsonDiscriminator(nameof(D_MultipleDependent), Required = true)]
    internal class D_MultipleDependentDocument : ID_MultipleDependentDocument, IMongoDbDocument<D_MultipleDependent, D_MultipleDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> DOptionalaggregatesIds { get; set; }

        public D_MultipleDependent ToEntity(D_MultipleDependent? entity = default)
        {
            entity ??= new D_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.DOptionalaggregatesIds = DOptionalaggregatesIds.ToList() ?? throw new Exception($"{nameof(entity.DOptionalaggregatesIds)} is null");

            return entity;
        }

        public D_MultipleDependentDocument PopulateFromEntity(D_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            DOptionalaggregatesIds = entity.DOptionalaggregatesIds.ToList();

            return this;
        }

        public static D_MultipleDependentDocument? FromEntity(D_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new D_MultipleDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<D_MultipleDependentDocument> GetIdFilter(string id)
        {
            return Builders<D_MultipleDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<D_MultipleDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<D_MultipleDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<D_MultipleDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<D_MultipleDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<D_MultipleDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}