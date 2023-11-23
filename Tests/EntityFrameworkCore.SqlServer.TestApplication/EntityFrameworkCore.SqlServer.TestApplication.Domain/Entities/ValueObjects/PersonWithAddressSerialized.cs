using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ValueObjects
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class PersonWithAddressSerialized : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AddressSerialized AddressSerialized { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}