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

        public Guid Id { get; set; }

        public string RequiredCompAttr { get; set; }

        public virtual A_OptionalDependent AOptionalDependent { get; set; }

        IA_OptionalDependent IA_RequiredComposite.AOptionalDependent
        {
            get => AOptionalDependent;
            set => AOptionalDependent = (A_OptionalDependent)value;
        }


    }
}
