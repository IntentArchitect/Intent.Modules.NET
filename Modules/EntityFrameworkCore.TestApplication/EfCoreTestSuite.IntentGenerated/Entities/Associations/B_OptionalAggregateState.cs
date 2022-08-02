using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class B_OptionalAggregate : IB_OptionalAggregate
    {
        public B_OptionalAggregate()
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

        private string _attribute;

        public string Attribute
        {
            get { return _attribute; }
            set
            {
                _attribute = value;
            }
        }

        private B_OptionalDependent _b_OptionalDependent;

        public virtual B_OptionalDependent B_OptionalDependent
        {
            get
            {
                return _b_OptionalDependent;
            }
            set
            {
                _b_OptionalDependent = value;
            }
        }

        public Guid? B_OptionalDependentId { get; set; }
    }
}
