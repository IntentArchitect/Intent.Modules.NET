using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational
{
    public class OptionalToManySource
    {
        private List<OptionalToManyDest> _optionalOneToManyDests = new List<OptionalToManyDest>();

        public OptionalToManySource(string attribute, IEnumerable<OptionalToManyDest> optionalOneToManyDests)
        {
            Attribute = attribute;
            _optionalOneToManyDests = new List<OptionalToManyDest>(optionalOneToManyDests);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
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