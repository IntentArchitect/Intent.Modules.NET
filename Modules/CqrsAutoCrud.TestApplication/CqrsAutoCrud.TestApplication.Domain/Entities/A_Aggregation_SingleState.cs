using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class A_Aggregation_Single : IA_Aggregation_Single
    {
        public A_Aggregation_Single()
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

        private AA3_Composite_Single _composite;

        public virtual AA3_Composite_Single Composite
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

        private ICollection<AA3_Composite_Many> _composites;

        public virtual ICollection<AA3_Composite_Many> Composites
        {
            get
            {
                return _composites ??= new List<AA3_Composite_Many>();
            }
            set
            {
                _composites = value;
            }
        }

        private AA3_Aggregation_Single _aggregation;

        public virtual AA3_Aggregation_Single Aggregation
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

        private ICollection<AA3_Aggregation_Many> _aggregations;

        public virtual ICollection<AA3_Aggregation_Many> Aggregations
        {
            get
            {
                return _aggregations ??= new List<AA3_Aggregation_Many>();
            }
            set
            {
                _aggregations = value;
            }
        }


    }
}
