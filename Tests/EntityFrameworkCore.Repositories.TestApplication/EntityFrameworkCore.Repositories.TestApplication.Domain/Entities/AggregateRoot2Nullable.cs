using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateRoot2Nullable
    {
        public Guid Id { get; set; }
    }
}