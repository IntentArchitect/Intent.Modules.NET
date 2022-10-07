using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_SecondLevel : IPoly_SecondLevel
    {
        public Poly_SecondLevel()
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

        private string _secondField;

        public string SecondField
        {
            get { return _secondField; }
            set
            {
                _secondField = value;
            }
        }

        private string _partitionKey;

        public string PartitionKey
        {
            get { return _partitionKey; }
            set
            {
                _partitionKey = value;
            }
        }

        private ICollection<Poly_BaseClassNonAbstract> _poly_BaseClassNonAbstracts;

        public virtual ICollection<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts
        {
            get
            {
                return _poly_BaseClassNonAbstracts ??= new List<Poly_BaseClassNonAbstract>();
            }
            set
            {
                _poly_BaseClassNonAbstracts = value;
            }
        }


    }
}
