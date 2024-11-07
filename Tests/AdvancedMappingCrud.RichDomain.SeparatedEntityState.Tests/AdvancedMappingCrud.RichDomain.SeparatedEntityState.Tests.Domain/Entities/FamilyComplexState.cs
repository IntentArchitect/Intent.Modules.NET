using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class FamilyComplex : IHasDomainEvent
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected FamilyComplex()
        {
            ChildName = null!;
            ParentName = null!;
            GreatGrandParentName = null!;
            AuntName = null!;
        }
        public Guid Id { get; set; }

        public string ChildName { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public long GrandParentId { get; set; }

        public long GreatGrandParentId { get; set; }

        public string GreatGrandParentName { get; set; }

        public int AuntId { get; set; }

        public string AuntName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}