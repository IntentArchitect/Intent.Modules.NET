using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class C_MultipleDependent : IC_MultipleDependent
    {
        public C_MultipleDependent()
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

        private string _multipleDependentAttr;

        public string MultipleDependentAttr
        {
            get { return _multipleDependentAttr; }
            set
            {
                _multipleDependentAttr = value;
            }
        }


        public Guid C_RequiredCompositeId { get; set; }
    }
}
