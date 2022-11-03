using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeManyBB : ICompositeManyBB
    {
        public CompositeManyBB()
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

        private Guid _aCompositeManyId;

        public Guid ACompositeManyId
        {
            get { return _aCompositeManyId; }
            set
            {
                _aCompositeManyId = value;
            }
        }

        public Guid A_Composite_ManyId { get; set; }
    }
}
