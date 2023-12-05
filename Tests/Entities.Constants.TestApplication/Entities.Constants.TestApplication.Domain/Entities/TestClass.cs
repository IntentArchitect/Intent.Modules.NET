using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.Constants.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TestClass : IHasDomainEvent
    {
        public const int Att100MaxLength = 100;
        public const int VarChar200MaxLength = 200;
        public const int NVarChar300MaxLength = 300;

        public Guid Id { get; set; }

        public string Att100 { get; set; }

        public string VarChar200 { get; set; }

        public string NVarChar300 { get; set; }

        public string AttMax { get; set; }

        public string VarCharMax { get; set; }

        public string NVarCharMax { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}