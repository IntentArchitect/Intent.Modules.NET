using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{

    public partial class ClassB : IClassB
    {

        public Guid Id { get; set; }

        public string ClassBAttr { get; set; }

        public virtual ClassC ClassC { get; set; }

        IClassC IClassB.ClassC
        {
            get => ClassC;
            set => ClassC = (ClassC)value;
        }

        public virtual ICollection<ClassD> ClassDS { get; set; } = new List<ClassD>();

        ICollection<IClassD> IClassB.ClassDS
        {
            get => ClassDS.CreateWrapper<IClassD, ClassD>();
            set => ClassDS = value.Cast<ClassD>().ToList();
        }
    }
}
