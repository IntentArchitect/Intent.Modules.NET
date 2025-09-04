using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.IdTypes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.IdTypes
{
    internal class IdTypeGuidDocument : IIdTypeGuidDocument, IMongoDbDocument<IdTypeGuid, IdTypeGuidDocument, Guid>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public IdTypeGuid ToEntity(IdTypeGuid? entity = default)
        {
            entity ??= new IdTypeGuid();

            entity.Id = Id;
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public IdTypeGuidDocument PopulateFromEntity(IdTypeGuid entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static IdTypeGuidDocument? FromEntity(IdTypeGuid? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new IdTypeGuidDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<IdTypeGuidDocument> GetIdFilter(Guid id)
        {
            return Builders<IdTypeGuidDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<IdTypeGuidDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<IdTypeGuidDocument> GetIdsFilter(Guid[] ids)
        {
            return Builders<IdTypeGuidDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<IdTypeGuidDocument, bool>> GetIdFilterPredicate(Guid id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<IdTypeGuidDocument, bool>> GetIdsFilterPredicate(Guid[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}