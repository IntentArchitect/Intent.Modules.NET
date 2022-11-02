using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class A_Aggregation_Many : IA_Aggregation_Many
    {
        public A_Aggregation_Many()
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

        private string _aggregationAttr;

        public string AggregationAttr
        {
            get { return _aggregationAttr; }
            set
            {
                _aggregationAttr = value;
            }
        }

        private Guid? _aAggregaterootId;

        public Guid? AAggregaterootId
        {
            get { return _aAggregaterootId; }
            set
            {
                _aAggregaterootId = value;
            }
        }

        private AA4_Composite_Single _composite;

        public virtual AA4_Composite_Single Composite
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

        private ICollection<AA4_Composite_Many> _composites;

        public virtual ICollection<AA4_Composite_Many> Composites
        {
            get
            {
                return _composites ??= new List<AA4_Composite_Many>();
            }
            set
            {
                _composites = value;
            }
        }

        private AA4_Aggregation_Single _aggregation;

        public virtual AA4_Aggregation_Single Aggregation
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

        private ICollection<AA4_Aggregation_Many> _aggregations;

        public virtual ICollection<AA4_Aggregation_Many> Aggregations
        {
            get
            {
                return _aggregations ??= new List<AA4_Aggregation_Many>();
            }
            set
            {
                _aggregations = value;
            }
        }

        public Guid? A_AggregateRootId { get; set; }
    }
}
