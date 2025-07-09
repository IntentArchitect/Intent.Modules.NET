using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class J_RequiredDependent : IHasDomainEvent
    {
        public J_RequiredDependent()
        {
            PartitionKey = null!;
            RequiredDepAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredDepAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}