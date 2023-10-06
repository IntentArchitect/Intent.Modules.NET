using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompositeManyAA
    {
        public CompositeManyAA()
        {
            CompositeAttr = null!;
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid CompositeSingleAId { get; set; }
    }
}