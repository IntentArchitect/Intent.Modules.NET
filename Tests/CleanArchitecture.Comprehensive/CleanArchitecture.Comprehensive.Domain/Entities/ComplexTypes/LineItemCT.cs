using System;
using CleanArchitecture.Comprehensive.Domain.ComplexTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class LineItemCT
    {
        public Guid Id { get; set; }

        public Guid OrderCTId { get; set; }

        public MoneyCT Money { get; set; }
    }
}