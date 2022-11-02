using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class A_Composite_Many : IA_Composite_Many
    {
        public A_Composite_Many()
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

        private AA2_Composite_Single _composite;

        public virtual AA2_Composite_Single Composite
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

        private ICollection<AA2_Composite_Many> _composites;

        public virtual ICollection<AA2_Composite_Many> Composites
        {
            get
            {
                return _composites ??= new List<AA2_Composite_Many>();
            }
            set
            {
                _composites = value;
            }
        }

        private AA2_Aggregation_Single _aggregation;

        public virtual AA2_Aggregation_Single Aggregation
        {
            get
            {
                return _aggregation;
            }
            set
            {
                _aggregation = value;
            }
        }
        public Guid? AggregationId { get; set; }

        public Guid A_AggregateRootId { get; set; }
    }
}
