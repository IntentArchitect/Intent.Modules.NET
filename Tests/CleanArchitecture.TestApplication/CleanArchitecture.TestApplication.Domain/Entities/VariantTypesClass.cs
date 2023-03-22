using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class VariantTypesClass : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public ICollection<string> StrCollection { get; set; } = new List<string>();

        public ICollection<int> IntCollection { get; set; } = new List<int>();

        public ICollection<string>? StrNullCollection { get; set; } = new List<string>();

        public ICollection<int>? IntNullCollection { get; set; } = new List<int>();

        public string? NullStr { get; set; }

        public int? NullInt { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}