using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class A_RequiredComposite : IA_RequiredComposite
    {

        public Guid Id
        { get; set; }

        public string Attribute
        { get; set; }

        public virtual A_OptionalDependent A_OptionalDependent
        { get; set; }

        IA_OptionalDependent IA_RequiredComposite.A_OptionalDependent
        {
            get => A_OptionalDependent;
            set => A_OptionalDependent = (A_OptionalDependent)value;
        }


    }
}
