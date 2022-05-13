using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class StereotypeIndex : IStereotypeIndex
    {
        public StereotypeIndex()
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

        private Guid _defaultIndexField;

        public Guid DefaultIndexField
        {
            get { return _defaultIndexField; }
            set
            {
                _defaultIndexField = value;
            }
        }

        private Guid _customIndexField;

        public Guid CustomIndexField
        {
            get { return _customIndexField; }
            set
            {
                _customIndexField = value;
            }
        }

        private Guid _groupedIndexFieldA;

        public Guid GroupedIndexFieldA
        {
            get { return _groupedIndexFieldA; }
            set
            {
                _groupedIndexFieldA = value;
            }
        }

        private Guid _groupedIndexFieldB;

        public Guid GroupedIndexFieldB
        {
            get { return _groupedIndexFieldB; }
            set
            {
                _groupedIndexFieldB = value;
            }
        }

    }
}
