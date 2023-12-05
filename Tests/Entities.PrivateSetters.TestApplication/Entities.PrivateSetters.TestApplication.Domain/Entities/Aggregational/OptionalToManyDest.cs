using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class OptionalToManyDest
    {
        public OptionalToManyDest(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OptionalToManyDest()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public Guid? OptionalOneToManySourceId { get; private set; }

        public string Attribute { get; private set; }

        public virtual OptionalToManySource? OptionalOneToManySource { get; private set; }

        public async Task OperationAsync(string attribute, CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
        }
    }
}