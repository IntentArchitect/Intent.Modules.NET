using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.EF8.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ValueObjects
{
    public class DictionaryWithKvPSerialized
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairSerialized> KeyValuePairSerializeds { get; set; } = new List<KeyValuePairSerialized>();
    }
}