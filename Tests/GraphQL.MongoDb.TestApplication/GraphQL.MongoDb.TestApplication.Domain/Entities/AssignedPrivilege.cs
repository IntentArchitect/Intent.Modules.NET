using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Entities
{
    public class AssignedPrivilege
    {
        private string? _id;

        public AssignedPrivilege()
        {
            Id = null!;
            PrivilegeId = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string PrivilegeId { get; set; }
    }
}