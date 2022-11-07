using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{
    public partial class Base : IBase
    {
        public Guid BaseAssociatedId { get; set; }

        public string BaseField1 { get; set; }

        public virtual BaseAssociated BaseAssociated { get; set; }

        IBaseAssociated IBase.BaseAssociated
        {
            get => BaseAssociated;
            set => BaseAssociated = (BaseAssociated)value;
        }
    }
}