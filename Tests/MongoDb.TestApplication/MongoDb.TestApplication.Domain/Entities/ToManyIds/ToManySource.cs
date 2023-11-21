using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.ToManyIds
{
    public class ToManySource
    {
        public string Id { get; set; }

        public ICollection<ToManyGuid> ToManyGuids { get; set; } = new List<ToManyGuid>();

        public ICollection<ToManyInt> ToManyInts { get; set; } = new List<ToManyInt>();

        public ICollection<ToManyLong> ToManyLongs { get; set; } = new List<ToManyLong>();
    }
}