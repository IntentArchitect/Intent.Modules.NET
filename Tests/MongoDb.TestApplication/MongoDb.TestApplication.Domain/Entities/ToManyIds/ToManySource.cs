using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.ToManyIds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ToManySource
    {
        public string Id { get; set; }

        public virtual ICollection<ToManyGuid> ToManyGuids { get; set; } = new List<ToManyGuid>();

        public virtual ICollection<ToManyInt> ToManyInts { get; set; } = new List<ToManyInt>();

        public virtual ICollection<ToManyLong> ToManyLongs { get; set; } = new List<ToManyLong>();
    }
}