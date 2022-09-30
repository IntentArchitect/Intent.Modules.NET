using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{

    public partial class ClassA : IClassA
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string ClassAAttr { get; set; }

        public virtual ICollection<ClassB> ClassBS { get; set; } = new List<ClassB>();

        ICollection<IClassB> IClassA.ClassBS
        {
            get => ClassBS.CreateWrapper<IClassB, ClassB>();
            set => ClassBS = value.Cast<ClassB>().ToList();
        }


    }
}
