using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_BaseClassNonAbstract : IPoly_BaseClassNonAbstract
    {
        public Poly_BaseClassNonAbstract()
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


        private string _baseField;

        public string BaseField
        {
            get { return _baseField; }
            set
            {
                _baseField = value;
            }
        }
    }
}
