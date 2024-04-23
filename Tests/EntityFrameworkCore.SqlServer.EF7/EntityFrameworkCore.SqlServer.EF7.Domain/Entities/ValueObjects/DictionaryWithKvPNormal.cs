using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.EF7.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.ValueObjects
{
    public class DictionaryWithKvPNormal
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairNormal> KeyValuePairNormals { get; set; } = new List<KeyValuePairNormal>();
    }
}