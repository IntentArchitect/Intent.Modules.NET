using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public partial class Derived : Base, IDerived
    {

        public string PartitionKey { get; set; }

        public string DerivedField1 { get; set; }


        public Guid AssociatedId { get; set; }

        public virtual Associated Associated { get; set; }

        IAssociated IDerived.Associated
        {
            get => Associated;
            set => Associated = (Associated)value;
        }

        public virtual ICollection<Composite> Composites { get; set; } = new List<Composite>();

        ICollection<IComposite> IDerived.Composites
        {
            get => Composites.CreateWrapper<IComposite, Composite>();
            set => Composites = value.Cast<Composite>().ToList();
        }


    }
}
