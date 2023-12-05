using System;
using System.Collections.Generic;
using System.Linq;
using Entities.PrivateSetters.MongoDb.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice
    {
        private List<string> _tagsIds = new List<string>();
        private List<Line> _lines = new List<Line>();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public Invoice(DateTime date, IEnumerable<LineDataContract> lines, IEnumerable<Tag> tags)
        {
            Date = date;
            _tagsIds = new List<string>(tags.Select(x => x.Id));
            _lines.AddRange(lines.Select(x => new Line(x.Description, x.Quantity)));
        }

        public string Id { get; private set; }

        public DateTime Date { get; private set; }

        public IReadOnlyCollection<string> TagsIds
        {
            get => _tagsIds.AsReadOnly();
            private set => _tagsIds = new List<string>(value);
        }

        public IReadOnlyCollection<Line> Lines
        {
            get => _lines.AsReadOnly();
            private set => _lines = new List<Line>(value);
        }
    }
}