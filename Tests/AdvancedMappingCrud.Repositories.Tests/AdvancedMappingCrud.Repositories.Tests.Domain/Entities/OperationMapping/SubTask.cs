using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping
{
    public class SubTask
    {
        public SubTask()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TaskItemId { get; set; }
    }
}