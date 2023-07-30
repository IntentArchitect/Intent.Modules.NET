using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class OptionalToManySource
    {
        private List<OptionalToManyDest> _optionalOneToManyDests = new List<OptionalToManyDest>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public OptionalToManySource(string attribute, IEnumerable<OptionalToManyDest> optionalOneToManyDests)
        {
            Attribute = attribute;
            _optionalOneToManyDests = new List<OptionalToManyDest>(optionalOneToManyDests);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected OptionalToManySource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<OptionalToManyDest> OptionalOneToManyDests
        {
            get => _optionalOneToManyDests.AsReadOnly();
            private set => _optionalOneToManyDests = new List<OptionalToManyDest>(value);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationAsync(
            IEnumerable<OptionalToManyDest> optionalOneToManyDests,
            string attribute,
            CancellationToken cancellationToken = default)
        {
            _optionalOneToManyDests.Clear();
            _optionalOneToManyDests.AddRange(optionalOneToManyDests);
            Attribute = attribute;
        }
    }
}