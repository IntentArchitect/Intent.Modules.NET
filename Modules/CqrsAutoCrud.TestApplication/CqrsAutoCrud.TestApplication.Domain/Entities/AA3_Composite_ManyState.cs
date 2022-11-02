using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AA3_Composite_Many : IAA3_Composite_Many
    {
        public AA3_Composite_Many()
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

        private Guid _aAggregationSingleId;

        public Guid AAggregationSingleId
        {
            get { return _aAggregationSingleId; }
            set
            {
                _aAggregationSingleId = value;
            }
        }

        public Guid A_Aggregation_SingleId { get; set; }
    }
}
