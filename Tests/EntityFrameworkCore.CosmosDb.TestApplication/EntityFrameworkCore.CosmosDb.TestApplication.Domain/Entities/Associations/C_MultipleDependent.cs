using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class C_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDependentAttr { get; set; }

        public Guid CRequiredcompositeId { get; set; }
    }
}