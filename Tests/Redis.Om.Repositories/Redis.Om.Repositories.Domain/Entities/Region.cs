using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class Region : IHasDomainEvent
    {
        private string? _id;
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<Country> Countries { get; set; } = new List<Country>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}