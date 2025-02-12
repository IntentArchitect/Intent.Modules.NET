using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace OutputCachingRedis.Tests.Domain.Entities
{
    public class Files : IHasDomainEvent
    {
        public Files()
        {
            Content = null!;
            ContentType = null!;
        }
        public Guid Id { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}