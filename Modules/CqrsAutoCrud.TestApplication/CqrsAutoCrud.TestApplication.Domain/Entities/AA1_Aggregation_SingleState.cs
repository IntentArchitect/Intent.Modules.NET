using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AA1_Aggregation_Single : IAA1_Aggregation_Single
    {
        public AA1_Aggregation_Single()
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


    }
}
