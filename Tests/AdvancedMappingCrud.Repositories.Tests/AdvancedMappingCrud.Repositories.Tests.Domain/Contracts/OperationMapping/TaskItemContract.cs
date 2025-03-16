using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.OperationMapping
{
    public record TaskItemContract
    {
        public TaskItemContract(string name, IEnumerable<SubTaskItemContract> subTasks)
        {
            Name = name;
            SubTasks = subTasks;
        }

        public string Name { get; init; }
        public IEnumerable<SubTaskItemContract> SubTasks { get; init; }
    }
}