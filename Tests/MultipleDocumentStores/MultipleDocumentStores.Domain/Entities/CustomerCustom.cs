using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Common;

[assembly: IntentTagModeImplicit]

namespace MultipleDocumentStores.Domain.Entities
{
    public class CustomerCustom : IHasDomainEvent
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}