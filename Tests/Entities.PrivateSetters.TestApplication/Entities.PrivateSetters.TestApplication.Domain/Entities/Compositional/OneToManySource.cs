using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class OneToManySource
    {
        private List<OneToManyDest> _owneds = new List<OneToManyDest>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public OneToManySource(string attribute, IEnumerable<OneToManyDest> owneds)
        {
            Attribute = attribute;
            _owneds = new List<OneToManyDest>(owneds);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
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

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation(string attribute, IEnumerable<OneToManyDest> owneds)
        {
            Attribute = attribute;
            _owneds.Clear();
            _owneds.AddRange(owneds);
        }
    }
}