using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeManyB : ICompositeManyB
    {
        public CompositeManyB()
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

        private DateTime? _someDate;

        public DateTime? SomeDate
        {
            get { return _someDate; }
            set
            {
                _someDate = value;
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

        private CompositeSingleBB? _composite;

        public virtual CompositeSingleBB? Composite
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

        private ICollection<CompositeManyBB> _composites;

        public virtual ICollection<CompositeManyBB> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyBB>();
            }
            set
            {
                _composites = value;
            }
        }

        public Guid A_AggregateRootId { get; set; }
    }
}
