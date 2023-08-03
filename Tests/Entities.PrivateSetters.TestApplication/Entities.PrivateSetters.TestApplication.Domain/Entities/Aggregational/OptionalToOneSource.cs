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
    public class OptionalToOneSource
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public OptionalToOneSource(string attribute, OptionalToOneDest optionalToOneDest)
        {
            Attribute = attribute;
            OptionalToOneDest = optionalToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected OptionalToOneSource()
        {
            Attribute = null!;
            OptionalToOneDest = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OptionalToOneDest OptionalToOneDest { get; private set; }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task OperationAsync(
            string attribute,
            OptionalToOneDest optionalToOneDest,
            CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
            OptionalToOneDest = optionalToOneDest;
        }
    }
}