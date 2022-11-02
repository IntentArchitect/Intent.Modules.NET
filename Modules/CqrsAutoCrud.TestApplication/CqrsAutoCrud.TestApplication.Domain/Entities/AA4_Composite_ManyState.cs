using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AA4_Composite_Many : IAA4_Composite_Many
    {
        public AA4_Composite_Many()
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

        private Guid _aAggregationManyId;

        public Guid AAggregationManyId
        {
            get { return _aAggregationManyId; }
            set
            {
                _aAggregationManyId = value;
            }
        }

        public Guid A_Aggregation_ManyId { get; set; }
    }
}
