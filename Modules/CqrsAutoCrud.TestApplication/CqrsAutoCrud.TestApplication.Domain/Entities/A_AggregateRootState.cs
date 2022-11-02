using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class A_AggregateRoot : IA_AggregateRoot
    {
        public A_AggregateRoot()
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

        private A_Composite_Single _composite;

        public virtual A_Composite_Single Composite
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

        private ICollection<A_Composite_Many> _composites;

        public virtual ICollection<A_Composite_Many> Composites
        {
            get
            {
                return _composites ??= new List<A_Composite_Many>();
            }
            set
            {
                _composites = value;
            }
        }

        private A_Aggregation_Single _aggregation;

        public virtual A_Aggregation_Single Aggregation
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

        private ICollection<A_Aggregation_Many> _aggregations;

        public virtual ICollection<A_Aggregation_Many> Aggregations
        {
            get
            {
                return _aggregations ??= new List<A_Aggregation_Many>();
            }
            set
            {
                _aggregations = value;
            }
        }


    }
}
