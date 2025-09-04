using System;
using System.Collections.Generic;
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
    internal class D_MultipleDependentDocument : ID_MultipleDependentDocument, IMongoDbDocument<D_MultipleDependent, D_MultipleDependentDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> DOptionalAggregatesIds { get; set; }

        public D_MultipleDependent ToEntity(D_MultipleDependent? entity = default)
        {
            entity ??= new D_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.DOptionalAggregatesIds = DOptionalAggregatesIds.ToList() ?? throw new Exception($"{nameof(entity.DOptionalAggregatesIds)} is null");

            return entity;
        }

        public D_MultipleDependentDocument PopulateFromEntity(D_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            DOptionalAggregatesIds = entity.DOptionalAggregatesIds.ToList();

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