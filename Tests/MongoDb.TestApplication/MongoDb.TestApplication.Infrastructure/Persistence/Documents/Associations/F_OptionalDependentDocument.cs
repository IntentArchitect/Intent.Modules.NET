using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class F_OptionalDependentDocument : IF_OptionalDependentDocument, IMongoDbDocument<F_OptionalDependent, F_OptionalDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string? FOptionalaggregatenavId { get; set; }

        public F_OptionalDependent ToEntity(F_OptionalDependent? entity = default)
        {
            entity ??= new F_OptionalDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.FOptionalaggregatenavId = FOptionalaggregatenavId;

            return entity;
        }

        public F_OptionalDependentDocument PopulateFromEntity(F_OptionalDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            FOptionalaggregatenavId = entity.FOptionalaggregatenavId;

            return this;
        }

        public static F_OptionalDependentDocument? FromEntity(F_OptionalDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new F_OptionalDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<F_OptionalDependentDocument> GetIdFilter(string id)
        {
            return Builders<F_OptionalDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<F_OptionalDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<F_OptionalDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<F_OptionalDependentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}