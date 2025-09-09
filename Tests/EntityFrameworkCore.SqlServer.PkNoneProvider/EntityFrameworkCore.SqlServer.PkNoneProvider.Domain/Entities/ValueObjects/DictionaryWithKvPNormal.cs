using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.ValueObjects
{
    public class DictionaryWithKvPNormal
    {
        public DictionaryWithKvPNormal()
        {
            Title = null!;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public ICollection<KeyValuePairNormal> KeyValuePairNormals { get; set; } = [];
    }
}