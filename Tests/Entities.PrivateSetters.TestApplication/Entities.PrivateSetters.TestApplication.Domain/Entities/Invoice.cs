using System;
using System.Collections.Generic;
using Entities.PrivateSetters.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice
    {
        private List<Line> _lines = new List<Line>();
        private List<Tag> _tags = new List<Tag>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public Invoice(DateTime date, IEnumerable<Tag> tags, IEnumerable<LineDataContract> lines)
        {
            Date = date;
            _tags = new List<Tag>(tags);
        }

        public DateTime Date { get; private set; }

        public virtual IReadOnlyCollection<Line> Lines
        {
            get => _lines.AsReadOnly();
            private set => _lines = new List<Line>(value);
        }

        public virtual IReadOnlyCollection<Tag> Tags
        {
            get => _tags.AsReadOnly();
            private set => _tags = new List<Tag>(value);
        }

        [IntentManaged(Mode.Fully)]
        public void Operation(DateTime date, IEnumerable<Tag> tags, IEnumerable<LineDataContract> lines)
        {
            Date = date;
            _tags.Clear();
            _tags.AddRange(tags);
        }
    }
}