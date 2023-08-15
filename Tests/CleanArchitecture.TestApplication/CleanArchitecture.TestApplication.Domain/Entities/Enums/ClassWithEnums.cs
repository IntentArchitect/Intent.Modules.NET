using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.Enums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ClassWithEnums : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public EnumWithDefaultLiteral EnumWithDefaultLiteral { get; set; }

        public EnumWithoutDefaultLiteral EnumWithoutDefaultLiteral { get; set; }

        public EnumWithoutValues EnumWithoutValues { get; set; }

        public EnumWithDefaultLiteral? NullibleEnumWithDefaultLiteral { get; set; }

        public EnumWithoutDefaultLiteral? NullibleEnumWithoutDefaultLiteral { get; set; }

        public EnumWithoutValues? NullibleEnumWithoutValues { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}