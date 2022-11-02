using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial class CompositeSingleAA : ICompositeSingleAA
    {
        public CompositeSingleAA()
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

        private CompositeSingleAAA1? _composite;

        public virtual CompositeSingleAAA1? Composite
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

        private ICollection<CompositeManyAAA1> _composites;

        public virtual ICollection<CompositeManyAAA1> Composites
        {
            get
            {
                return _composites ??= new List<CompositeManyAAA1>();
            }
            set
            {
                _composites = value;
            }
        }


    }
}
