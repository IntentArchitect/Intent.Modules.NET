using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class ComplexDefaultIndex : IComplexDefaultIndex
    {
        public ComplexDefaultIndex()
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

        private Guid _fieldA;

        public Guid FieldA
        {
            get { return _fieldA; }
            set
            {
                _fieldA = value;
            }
        }

        private Guid _fieldB;

        public Guid FieldB
        {
            get { return _fieldB; }
            set
            {
                _fieldB = value;
            }
        }

    }
}
