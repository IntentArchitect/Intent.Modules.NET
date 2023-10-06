using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ValueObjects
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class DictionaryWithKvPSerialized : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairSerialized> KeyValuePairSerializeds { get; set; } = new List<KeyValuePairSerialized>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}