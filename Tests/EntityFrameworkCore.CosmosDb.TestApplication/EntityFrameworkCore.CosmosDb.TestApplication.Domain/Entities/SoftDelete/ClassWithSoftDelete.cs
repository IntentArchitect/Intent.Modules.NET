using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.SoftDelete
{
    public class ClassWithSoftDelete : IHasDomainEvent, ISoftDelete
    {
        public ClassWithSoftDelete()
        {
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public bool IsDeleted { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}