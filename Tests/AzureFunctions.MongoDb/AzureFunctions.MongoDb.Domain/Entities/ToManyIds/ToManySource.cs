using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.ToManyIds
{
    public class ToManySource
    {
        public ToManySource()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public ICollection<ToManyGuid> ToManyGuids { get; set; } = [];

        public ICollection<ToManyInt> ToManyInts { get; set; } = [];

        public ICollection<ToManyLong> ToManyLongs { get; set; } = [];
    }
}