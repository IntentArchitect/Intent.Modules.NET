using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class C_RequireCompositeDocument : IC_RequireCompositeDocument, IMongoDbDocument<C_RequireComposite, C_RequireCompositeDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<IC_MultipleDependentDocument> C_MultipleDependents { get; set; }

        public C_RequireComposite ToEntity(C_RequireComposite? entity = default)
        {
            entity ??= new C_RequireComposite();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.C_MultipleDependents = C_MultipleDependents.Select(x => (x as C_MultipleDependentDocument).ToEntity()).ToList();

            return entity;
        }

        public C_RequireCompositeDocument PopulateFromEntity(C_RequireComposite entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            C_MultipleDependents = entity.C_MultipleDependents.Select(x => C_MultipleDependentDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static C_RequireCompositeDocument? FromEntity(C_RequireComposite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new C_RequireCompositeDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<C_RequireCompositeDocument> GetIdFilter(string id)
        {
            return Builders<C_RequireCompositeDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<C_RequireCompositeDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<C_RequireCompositeDocument> GetIdsFilter(string[] ids)
        {
            return Builders<C_RequireCompositeDocument>.Filter.In(d => d.Id, ids);
        }
    }
}