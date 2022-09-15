using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities
{

    public abstract partial class Base : IBase
    {
        public Base()
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

        private string _baseField1;

        public string BaseField1
        {
            get { return _baseField1; }
            set
            {
                _baseField1 = value;
            }
        }


        public Guid BaseAssociatedId { get; set; }
        private BaseAssociated _baseAssociated;

        public virtual BaseAssociated BaseAssociated
        {
            get
            {
                return _baseAssociated;
            }
            set
            {
                _baseAssociated = value;
            }
        }


    }
}
