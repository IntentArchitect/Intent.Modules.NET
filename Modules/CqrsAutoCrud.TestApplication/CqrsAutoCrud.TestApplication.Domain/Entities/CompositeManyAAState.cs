using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeManyAA : ICompositeManyAA
    {
        public CompositeManyAA()
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

        private Guid _aCompositeSingleId;

        public Guid ACompositeSingleId
        {
            get { return _aCompositeSingleId; }
            set
            {
                _aCompositeSingleId = value;
            }
        }

        public Guid A_Composite_SingleId { get; set; }
    }
}
