using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record EntityRecord
    {
        public EntityRecord(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}