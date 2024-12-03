using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class CustomConstructor : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Col1 { get; set; }

        public string Col2 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}