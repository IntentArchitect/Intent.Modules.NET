using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class A_Composite_Single : IA_Composite_Single
    {
        public A_Composite_Single()
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

        private string _compositeAttr;

        public string CompositeAttr
        {
            get { return _compositeAttr; }
            set
            {
                _compositeAttr = value;
            }
        }

        private AA1_Composite_Single _composite;

        public virtual AA1_Composite_Single Composite
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

        private ICollection<AA1_Composite_Many> _composites;

        public virtual ICollection<AA1_Composite_Many> Composites
        {
            get
            {
                return _composites ??= new List<AA1_Composite_Many>();
            }
            set
            {
                _composites = value;
            }
        }

        private AA1_Aggregation_Single _aggregation;

        public virtual AA1_Aggregation_Single Aggregation
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


    }
}
