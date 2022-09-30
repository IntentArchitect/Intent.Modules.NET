using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    [IntentManaged(Mode.Merge)]
    [DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore, Targets = Targets.Methods, AccessModifiers = AccessModifiers.Public)]
    public partial class K_SelfReference : IK_SelfReference
    {

    }
}