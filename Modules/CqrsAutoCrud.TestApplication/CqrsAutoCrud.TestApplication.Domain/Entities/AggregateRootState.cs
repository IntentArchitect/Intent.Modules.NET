using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AggregateRoot : IAggregateRoot
    {
        public AggregateRoot()
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

        private CompositeSingleA? _composite;

        public virtual CompositeSingleA? Composite
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

        private ICollection<CompositeManyB> _composites;

        public virtual ICollection<CompositeManyB> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyB>();
            }
            set
            {
                _composites = value;
            }
        }

        private AggregateSingleC? _aggregate;

        public virtual AggregateSingleC? Aggregate
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
