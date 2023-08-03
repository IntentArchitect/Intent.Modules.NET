using System;
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
    public class ManyToOneSource
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public ManyToOneSource(string attribute, ManyToOneDest manyToOneDest)
        {
            Attribute = attribute;
            ManyToOneDest = manyToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected ManyToOneSource()
        {
            Attribute = null!;
            ManyToOneDest = null!;
        }

        public Guid Id { get; private set; }

        public Guid ManyToOneDestId { get; private set; }

        public string Attribute { get; private set; }

        public virtual ManyToOneDest ManyToOneDest { get; private set; }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationAsync(
            string attribute,
            ManyToOneDest manyToOneDest,
            CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
            ManyToOneDest = manyToOneDest;
        }
    }
}