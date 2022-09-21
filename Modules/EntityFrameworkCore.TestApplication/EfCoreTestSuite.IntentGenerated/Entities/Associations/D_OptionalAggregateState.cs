using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class D_OptionalAggregate : ID_OptionalAggregate
    {
        public D_OptionalAggregate()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private ICollection<D_MultipleDependent> _d_MultipleDependents;

        public virtual ICollection<D_MultipleDependent> D_MultipleDependents
        {
            get
            {
                return _d_MultipleDependents ??= new List<D_MultipleDependent>();
            }
            set
            {
                _d_MultipleDependents = value;
            }
        }


    }
}
