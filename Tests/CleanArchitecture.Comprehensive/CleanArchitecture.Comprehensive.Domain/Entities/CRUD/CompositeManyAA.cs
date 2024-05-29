using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompositeManyAA
    {

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid CompositeSingleAId { get; set; }
    }
}