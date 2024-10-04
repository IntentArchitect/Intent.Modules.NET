using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Company : IHasDomainEvent
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected Company()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<ContactDetailsVO> ContactDetailsVOS { get; set; } = new List<ContactDetailsVO>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}