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
    [BsonDiscriminator(nameof(G_RequiredCompositeNav), Required = true)]
    internal class G_RequiredCompositeNavDocument : IG_RequiredCompositeNavDocument, IMongoDbDocument<G_RequiredCompositeNav, G_RequiredCompositeNavDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<IG_MultipleDependentDocument> G_MultipleDependents { get; set; }

        public G_RequiredCompositeNav ToEntity(G_RequiredCompositeNav? entity = default)
        {
            entity ??= new G_RequiredCompositeNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.G_MultipleDependents = G_MultipleDependents.Select(x => (x as G_MultipleDependentDocument).ToEntity()).ToList();

            return entity;
        }

        public G_RequiredCompositeNavDocument PopulateFromEntity(G_RequiredCompositeNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            G_MultipleDependents = entity.G_MultipleDependents.Select(x => G_MultipleDependentDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static G_RequiredCompositeNavDocument? FromEntity(G_RequiredCompositeNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new G_RequiredCompositeNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<G_RequiredCompositeNavDocument> GetIdFilter(string id)
        {
            return Builders<G_RequiredCompositeNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<G_RequiredCompositeNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<G_RequiredCompositeNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<G_RequiredCompositeNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<G_RequiredCompositeNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<G_RequiredCompositeNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}