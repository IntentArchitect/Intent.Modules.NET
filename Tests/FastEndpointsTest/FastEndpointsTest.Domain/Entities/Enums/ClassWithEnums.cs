using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.Enums
{
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