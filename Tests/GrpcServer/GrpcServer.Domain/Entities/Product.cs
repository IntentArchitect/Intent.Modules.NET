using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Domain.Common;
using GrpcServer.Domain.Services;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GrpcServer.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public async Task ApplyTagsAsync(
    IEnumerable<string> tagNames,
    ITagService tagService,
    CancellationToken cancellationToken = default)
        {
            // [IntentFully]
            // TODO: Implement ApplyTagsAsync (Product) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}