using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping
{
    public class TaskItem
    {
        public TaskItem()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TaskListId { get; set; }

        public virtual ICollection<SubTask> SubTasks { get; set; } = [];
    }
}