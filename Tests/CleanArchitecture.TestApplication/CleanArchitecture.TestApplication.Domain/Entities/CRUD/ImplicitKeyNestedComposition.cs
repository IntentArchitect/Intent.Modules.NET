using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ImplicitKeyNestedComposition
    {

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public Guid ImplicitKeyAggrRootId { get; set; }
    }
}