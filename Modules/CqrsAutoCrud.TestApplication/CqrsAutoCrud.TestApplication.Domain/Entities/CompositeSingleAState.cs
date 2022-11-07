using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeSingleA : ICompositeSingleA
    {
        public CompositeSingleA()
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

        private CompositeSingleAA? _composite;

        public virtual CompositeSingleAA? Composite
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

        private ICollection<CompositeManyAA> _composites;

        public virtual ICollection<CompositeManyAA> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyAA>();
            }
            set
            {
                _composites = value;
            }
        }


    }
}
