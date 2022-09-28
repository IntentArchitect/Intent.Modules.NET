using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{

    public partial class ClassB : IClassB
    {
        public ClassB()
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

        private string _classBAttr;

        public string ClassBAttr
        {
            get { return _classBAttr; }
            set
            {
                _classBAttr = value;
            }
        }

        private ClassC _classC;

        public virtual ClassC ClassC
        {
            get
            {
                return _classC;
            }
            set
            {
                _classC = value;
            }
        }

        private ICollection<ClassD> _classDS;

        public virtual ICollection<ClassD> ClassDS
        {
            get
            {
                return _classDS ??= new List<ClassD>();
            }
            set
            {
                _classDS = value;
            }
        }


        public Guid ClassAId { get; set; }
    }
}
