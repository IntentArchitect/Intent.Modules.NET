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
    internal class CompoundIndexEntityMultiParentDocument : ICompoundIndexEntityMultiParentDocument, IMongoDbDocument<CompoundIndexEntityMultiParent, CompoundIndexEntityMultiParentDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public IEnumerable<ICompoundIndexEntityMultiChildDocument> CompoundIndexEntityMultiChild { get; set; }

        public CompoundIndexEntityMultiParent ToEntity(CompoundIndexEntityMultiParent? entity = default)
        {
            entity ??= new CompoundIndexEntityMultiParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.CompoundIndexEntityMultiChild = CompoundIndexEntityMultiChild.Select(x => (x as CompoundIndexEntityMultiChildDocument).ToEntity()).ToList();

            return entity;
        }

        public CompoundIndexEntityMultiParentDocument PopulateFromEntity(CompoundIndexEntityMultiParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            CompoundIndexEntityMultiChild = entity.CompoundIndexEntityMultiChild.Select(x => CompoundIndexEntityMultiChildDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static CompoundIndexEntityMultiParentDocument? FromEntity(CompoundIndexEntityMultiParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompoundIndexEntityMultiParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CompoundIndexEntityMultiParentDocument> GetIdFilter(string id)
        {
            return Builders<CompoundIndexEntityMultiParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CompoundIndexEntityMultiParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CompoundIndexEntityMultiParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CompoundIndexEntityMultiParentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<CompoundIndexEntityMultiParentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CompoundIndexEntityMultiParentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}