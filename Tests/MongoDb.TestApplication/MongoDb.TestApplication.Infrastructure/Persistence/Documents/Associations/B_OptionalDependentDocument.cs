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
    internal class B_OptionalDependentDocument : IB_OptionalDependentDocument, IMongoDbDocument<B_OptionalDependent, B_OptionalDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public B_OptionalDependent ToEntity(B_OptionalDependent? entity = default)
        {
            entity ??= new B_OptionalDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public B_OptionalDependentDocument PopulateFromEntity(B_OptionalDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static B_OptionalDependentDocument? FromEntity(B_OptionalDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new B_OptionalDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<B_OptionalDependentDocument> GetIdFilter(string id)
        {
            return Builders<B_OptionalDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<B_OptionalDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<B_OptionalDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<B_OptionalDependentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}