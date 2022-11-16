using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IK_SelfReference
    {

        string SelfRefAttr { get; set; }

        IK_SelfReference K_SelfReferenceAssociation { get; set; }

    }
}
