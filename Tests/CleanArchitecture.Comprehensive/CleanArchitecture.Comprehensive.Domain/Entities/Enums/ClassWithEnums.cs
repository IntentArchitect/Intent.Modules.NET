using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.Enums
{
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