using System;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    internal class PrivilegeDocument : IPrivilegeDocument, IMongoDbDocument<Privilege, PrivilegeDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public Privilege ToEntity(Privilege? entity = default)
        {
            entity ??= new Privilege();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Description = Description;

            return entity;
        }

        public PrivilegeDocument PopulateFromEntity(Privilege entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;

            return this;
        }

        public static PrivilegeDocument? FromEntity(Privilege? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PrivilegeDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<PrivilegeDocument> GetIdFilter(string id)
        {
            return Builders<PrivilegeDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<PrivilegeDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<PrivilegeDocument> GetIdsFilter(string[] ids)
        {
            return Builders<PrivilegeDocument>.Filter.In(d => d.Id, ids);
        }
    }
}