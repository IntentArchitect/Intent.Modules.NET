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
    public class ManyToManySource
    {
        private List<ManyToManyDest> _manyToManyDests = new List<ManyToManyDest>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public ManyToManySource(string attribute, IEnumerable<ManyToManyDest> manyToManyDests)
        {
            Attribute = attribute;
            _manyToManyDests = new List<ManyToManyDest>(manyToManyDests);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected ManyToManySource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<ManyToManyDest> ManyToManyDests
        {
            get => _manyToManyDests.AsReadOnly();
            private set => _manyToManyDests = new List<ManyToManyDest>(value);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationAsync(
            string attribute,
            IEnumerable<ManyToManyDest> manyToManyDests,
            CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
            _manyToManyDests.Clear();
            _manyToManyDests.AddRange(manyToManyDests);
        }
    }
}