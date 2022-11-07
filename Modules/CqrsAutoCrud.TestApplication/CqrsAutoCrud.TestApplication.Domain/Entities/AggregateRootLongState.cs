using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class AggregateRootLong : IAggregateRootLong
    {
        public AggregateRootLong()
        {
        }


        private long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        private string _attribute;

        public string Attribute
        {
            get { return _attribute; }
            set
            {
                _attribute = value;
            }
        }

        private CompositeOfAggrLong? _compositeOfAggrLong;

        public virtual CompositeOfAggrLong? CompositeOfAggrLong
        {
            get
            {
                return _compositeOfAggrLong;
            }
            set
            {
                _compositeOfAggrLong = value;
            }
        }


    }
}
