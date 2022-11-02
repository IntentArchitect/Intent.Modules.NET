using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeManyAA : ICompositeManyAA
    {
        public CompositeManyAA()
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

        private Guid _aAggregaterootId;

        public Guid AAggregaterootId
        {
            get { return _aAggregaterootId; }
            set
            {
                _aAggregaterootId = value;
            }
        }

        private string _compositeAttr;

        public string CompositeAttr
        {
            get { return _compositeAttr; }
            set
            {
                _compositeAttr = value;
            }
        }

        private CompositeSingleAAA2? _composite;

        public virtual CompositeSingleAAA2? Composite
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

        private ICollection<CompositeManyAAA2> _composites;

        public virtual ICollection<CompositeManyAAA2> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyAAA2>();
            }
            set
            {
                _composites = value;
            }
        }

        public Guid A_AggregateRootId { get; set; }
    }
}
