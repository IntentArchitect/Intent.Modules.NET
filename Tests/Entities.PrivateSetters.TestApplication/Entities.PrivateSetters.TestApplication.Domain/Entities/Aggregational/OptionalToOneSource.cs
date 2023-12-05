using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class OptionalToOneSource
    {
        public OptionalToOneSource(string attribute, OptionalToOneDest optionalToOneDest)
        {
            Attribute = attribute;
            OptionalToOneDest = optionalToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OptionalToOneSource()
        {
            Attribute = null!;
            OptionalToOneDest = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public Guid OptionalToOneDestId { get; private set; }

        public virtual OptionalToOneDest OptionalToOneDest { get; private set; }

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