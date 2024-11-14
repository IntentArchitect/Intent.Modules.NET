using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class ChildSimple : IHasDomainEvent
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected ChildSimple()
        {
            ChildName = null!;
            ParentName = null!;
        }
        public Guid Id { get; set; }

        public string ChildName { get; set; }

        public string ParentName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}