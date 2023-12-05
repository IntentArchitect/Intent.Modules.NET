using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class ManyToOneDest
    {
        private List<ManyToOneSource> _manyToOneSources = new List<ManyToOneSource>();

        public ManyToOneDest(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ManyToOneDest()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<ManyToOneSource> ManyToOneSources
        {
            get => _manyToOneSources.AsReadOnly();
            private set => _manyToOneSources = new List<ManyToOneSource>(value);
        }

        public async Task OperationAsync(string attribute, CancellationToken cancellationToken = default)
        {
            Attribute = attribute;
        }
    }
}