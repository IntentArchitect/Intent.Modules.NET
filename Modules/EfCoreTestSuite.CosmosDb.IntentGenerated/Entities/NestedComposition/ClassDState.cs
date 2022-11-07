using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{
    public partial class ClassD : IClassD
    {
        public Guid Id { get; set; }

        public virtual ClassE ClassE { get; set; }

        IClassE IClassD.ClassE
        {
            get => ClassE;
            set => ClassE = (ClassE)value;
        }
    }
}