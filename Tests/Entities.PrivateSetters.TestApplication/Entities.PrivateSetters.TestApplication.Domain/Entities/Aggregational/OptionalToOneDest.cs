using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class OptionalToOneDest
    {
        public OptionalToOneDest(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OptionalToOneDest()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OptionalToOneSource? OptionalToOneSource { get; private set; }

        public async Task OperationAsync(
            string attribute,
            OptionalToOneSource? optionalToOneSource,
            CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
            OptionalToOneSource = optionalToOneSource;
        }
    }
}