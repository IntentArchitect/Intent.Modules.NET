using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Entities
{
    public class Privilege : IHasDomainEvent
    {
        public Privilege()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}