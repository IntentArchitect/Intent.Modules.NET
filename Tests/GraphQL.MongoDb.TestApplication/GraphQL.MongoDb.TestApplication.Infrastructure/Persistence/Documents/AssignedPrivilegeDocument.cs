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
    internal class AssignedPrivilegeDocument : IAssignedPrivilegeDocument
    {
        public string Id { get; set; } = default!;
        public string PrivilegeId { get; set; } = default!;

        public AssignedPrivilege ToEntity(AssignedPrivilege? entity = default)
        {
            entity ??= new AssignedPrivilege();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PrivilegeId = PrivilegeId ?? throw new Exception($"{nameof(entity.PrivilegeId)} is null");

            return entity;
        }

        public AssignedPrivilegeDocument PopulateFromEntity(AssignedPrivilege entity)
        {
            Id = entity.Id;
            PrivilegeId = entity.PrivilegeId;

            return this;
        }

        public static AssignedPrivilegeDocument? FromEntity(AssignedPrivilege? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssignedPrivilegeDocument().PopulateFromEntity(entity);
        }
    }
}