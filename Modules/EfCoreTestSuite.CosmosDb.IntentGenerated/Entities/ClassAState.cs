using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities
{

    public partial class ClassA : IClassA
    {
        public ClassA()
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

        private string _partitionKey;

        public string PartitionKey
        {
            get { return _partitionKey; }
            set
            {
                _partitionKey = value;
            }
        }

        private string _classAAttr;

        public string ClassAAttr
        {
            get { return _classAAttr; }
            set
            {
                _classAAttr = value;
            }
        }

        private ICollection<ClassB> _classBS;

        public virtual ICollection<ClassB> ClassBS
        {
            get
            {
                return _classBS ??= new List<ClassB>();
            }
            set
            {
                _classBS = value;
            }
        }


    }
}
