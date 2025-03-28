using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class BaseType : IHasDomainEvent
    {
        private string? _id;
        public BaseType()
        {
            Id = null!;
            BaseName = null!;
        }
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string BaseName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}