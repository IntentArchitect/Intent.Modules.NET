using System;
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
    internal class J_MultipleDependentDocument : IJ_MultipleDependentDocument, IMongoDbDocument<J_MultipleDependent, J_MultipleDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public J_MultipleDependent ToEntity(J_MultipleDependent? entity = default)
        {
            entity ??= new J_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public J_MultipleDependentDocument PopulateFromEntity(J_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static J_MultipleDependentDocument? FromEntity(J_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new J_MultipleDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<J_MultipleDependentDocument> GetIdFilter(string id)
        {
            return Builders<J_MultipleDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<J_MultipleDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<J_MultipleDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<J_MultipleDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<J_MultipleDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<J_MultipleDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}