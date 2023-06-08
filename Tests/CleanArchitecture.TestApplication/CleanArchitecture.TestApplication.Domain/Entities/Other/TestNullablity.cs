using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Other;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.Other
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TestNullablity : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public TestNullablity(Guid id,
            MyEnum myEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            MyEnum? nullableEnum)
        {
            Id = id;
            MyEnum = myEnum;
            Str = str;
            Date = date;
            DateTime = dateTime;
            NullableGuid = nullableGuid;
            NullableEnum = nullableEnum;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected TestNullablity()
        {
            Str = null!;
        }

        public Guid Id { get; set; }

        public MyEnum MyEnum { get; set; }

        public string Str { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateTime { get; set; }

        public Guid? NullableGuid { get; set; }

        public MyEnum? NullableEnum { get; set; }

        public Guid NullabilityPeerId { get; set; }

        public virtual ICollection<TestNullablityChild> TestNullablityChildren { get; set; } = new List<TestNullablityChild>();

        public virtual NullabilityPeer NullabilityPeer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}