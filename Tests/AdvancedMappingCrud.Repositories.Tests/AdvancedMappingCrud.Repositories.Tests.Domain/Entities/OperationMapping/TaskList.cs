using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping
{
    public class TaskList
    {
        public TaskList()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public virtual ICollection<TaskItem> TaskItems { get; set; } = [];
    }
}