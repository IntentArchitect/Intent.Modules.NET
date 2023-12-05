using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToManySource
    {
        private List<OneToManyDest> _owneds = new List<OneToManyDest>();

        public OneToManySource(string attribute, IEnumerable<OneToManyDest> owneds)
        {
            Attribute = attribute;
            _owneds = new List<OneToManyDest>(owneds);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToManySource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual IReadOnlyCollection<OneToManyDest> Owneds
        {
            get => _owneds.AsReadOnly();
            private set => _owneds = new List<OneToManyDest>(value);
        }

        public void Operation(string attribute, IEnumerable<OneToManyDest> owneds)
        {
            Attribute = attribute;
            _owneds.Clear();
            _owneds.AddRange(owneds);
        }
    }
}