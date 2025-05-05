using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.UpdateTaskItem
{
    public class UpdateTaskItemCommand : IRequest, ICommand
    {
        public UpdateTaskItemCommand(Guid userId, Guid taskListId, Guid id, string name)
        {
            UserId = userId;
            TaskListId = taskListId;
            Id = id;
            Name = name;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}