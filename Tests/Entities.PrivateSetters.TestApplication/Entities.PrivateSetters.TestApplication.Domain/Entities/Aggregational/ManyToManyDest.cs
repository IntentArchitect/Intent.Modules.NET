using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class ManyToManyDest
    {
        private List<ManyToManySource> _manyToManySources = new List<ManyToManySource>();

        public ManyToManyDest(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ManyToManyDest()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<ManyToManySource> ManyToManySources
        {
            get => _manyToManySources.AsReadOnly();
            private set => _manyToManySources = new List<ManyToManySource>(value);
        }

        public async Task OperationAsync(string attribute, CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
        }
    }
}