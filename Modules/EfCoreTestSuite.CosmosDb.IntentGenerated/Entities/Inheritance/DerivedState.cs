using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public partial class Derived : Base, IDerived
    {
        public Derived()
        {
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

        private string _derivedField1;

        public string DerivedField1
        {
            get { return _derivedField1; }
            set
            {
                _derivedField1 = value;
            }
        }


        public Guid AssociatedId
        { get; set; }
        private Associated _associated;

        public virtual Associated Associated
        {
            get
            {
                return _associated;
            }
            set
            {
                _associated = value;
            }
        }

        private ICollection<Composite> _composites;

        public virtual ICollection<Composite> Composites
        {
            get
            {
                return _composites ??= new List<Composite>();
            }
            set
            {
                _composites = value;
            }
        }


    }
}
