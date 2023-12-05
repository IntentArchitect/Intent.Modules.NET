using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class ManyToOneSource
    {
        public ManyToOneSource(string attribute, ManyToOneDest manyToOneDest)
        {
            Attribute = attribute;
            ManyToOneDest = manyToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ManyToOneSource()
        {
            Attribute = null!;
            ManyToOneDest = null!;
        }

        public Guid Id { get; private set; }

        public Guid ManyToOneDestId { get; private set; }

        public string Attribute { get; private set; }

        public virtual ManyToOneDest ManyToOneDest { get; private set; }

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