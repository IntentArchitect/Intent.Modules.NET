using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities
{

    public partial class FkAssociatedClass : IFkAssociatedClass
    {
        public FkAssociatedClass()
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

        public Guid FkDerivedClassCompositeKeyA { get; set; }
        public Guid FkDerivedClassCompositeKeyB { get; set; }
        private FkDerivedClass _fkDerivedClass;

        public virtual FkDerivedClass FkDerivedClass
        {
            get
            {
                return _fkDerivedClass;
            }
            set
            {
                _fkDerivedClass = value;
            }
        }


    }
}
