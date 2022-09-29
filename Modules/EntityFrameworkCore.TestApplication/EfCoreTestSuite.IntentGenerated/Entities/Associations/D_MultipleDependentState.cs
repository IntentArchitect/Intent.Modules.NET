using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class D_MultipleDependent : ID_MultipleDependent
    {
        public D_MultipleDependent()
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

        private string _multipleDepAttr;

        public string MultipleDepAttr
        {
            get { return _multipleDepAttr; }
            set
            {
                _multipleDepAttr = value;
            }
        }


        public Guid? D_OptionalAggregateId { get; set; }
    }
}
