using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.Nullability
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TestNullablityChild
    {
        public Guid Id { get; set; }

        public Guid TestNullablityId { get; set; }
    }
}