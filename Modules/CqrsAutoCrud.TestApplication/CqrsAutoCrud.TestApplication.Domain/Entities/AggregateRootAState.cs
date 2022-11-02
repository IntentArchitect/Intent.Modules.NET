using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AggregateRootA : IAggregateRootA
    {
        public AggregateRootA()
        {
        }


        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        private string _aggregateAttr;

        public string AggregateAttr
        {
            get { return _aggregateAttr; }
            set
            {
                _aggregateAttr = value;
            }
        }

        private CompositeSingleAA? _composite;

        public virtual CompositeSingleAA? Composite
        {
            get
            {
                return _composite;
            }
            set
            {
                _composite = value;
            }
        }

        private ICollection<CompositeManyAA> _composites;

        public virtual ICollection<CompositeManyAA> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyAA>();
            }
            set
            {
                _composites = value;
            }
        }

        private AggregateSingleAA? _aggregate;

        public virtual AggregateSingleAA? Aggregate
        {
            get
            {
                return _aggregate;
            }
            set
            {
                _aggregate = value;
            }
        }
        public Guid? AggregateId { get; set; }


    }
}
