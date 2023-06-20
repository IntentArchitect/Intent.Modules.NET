using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.Nullability
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TestNullablity : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public TestNullablity(Guid id,
            NoDefaultLiteralEnum sampleEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            NoDefaultLiteralEnum? nullableEnum,
            DefaultLiteralEnum defaultLiteralEnum)
        {
            Id = id;
            SampleEnum = sampleEnum;
            Str = str;
            Date = date;
            DateTime = dateTime;
            NullableGuid = nullableGuid;
            NullableEnum = nullableEnum;
            DefaultLiteralEnum = defaultLiteralEnum;
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

        public NoDefaultLiteralEnum SampleEnum { get; set; }

        public string Str { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateTime { get; set; }

        public Guid? NullableGuid { get; set; }

        public NoDefaultLiteralEnum? NullableEnum { get; set; }

        public Guid NullabilityPeerId { get; set; }

        public DefaultLiteralEnum DefaultLiteralEnum { get; set; }

        public virtual ICollection<TestNullablityChild> TestNullablityChildren { get; set; } = new List<TestNullablityChild>();

        public virtual NullabilityPeer NullabilityPeer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}