using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.OperationMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping
{
    public class User : IHasDomainEvent
    {
        public User()
        {
            UserName = null!;
        }

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<TaskList> TaskLists { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void AddTask(string listName, TaskItem task)
        {
            // [IntentFully]
            // TODO: Implement AddTask (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }

        public void AddTask(string listName, TaskItemContract task)
        {
            // [IntentFully]
            // TODO: Implement AddTask (User) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}