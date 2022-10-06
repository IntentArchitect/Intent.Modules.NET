using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract_Comp : IPoly_RootAbstract_Comp
    {
        public Poly_RootAbstract_Comp()
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

        private string _compField;

        public string CompField
        {
            get { return _compField; }
            set
            {
                _compField = value;
            }
        }


    }
}
