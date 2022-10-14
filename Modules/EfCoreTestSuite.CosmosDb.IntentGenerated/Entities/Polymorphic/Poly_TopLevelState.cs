using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_TopLevel : IPoly_TopLevel
    {
        public Poly_TopLevel()
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

        private string _topField;

        public string TopField
        {
            get { return _topField; }
            set
            {
                _topField = value;
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

        private ICollection<Poly_RootAbstract> _poly_RootAbstracts;

        public virtual ICollection<Poly_RootAbstract> Poly_RootAbstracts
        {
            get
            {
                return _poly_RootAbstracts ??= new List<Poly_RootAbstract>();
            }
            set
            {
                _poly_RootAbstracts = value;
            }
        }


    }
}
