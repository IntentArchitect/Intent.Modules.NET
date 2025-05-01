using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.DeleteTaskItem
{
    public class DeleteTaskItemCommand : IRequest, ICommand
    {
        public DeleteTaskItemCommand(Guid userId, Guid taskListId, Guid id)
        {
            UserId = userId;
            TaskListId = taskListId;
            Id = id;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public Guid Id { get; set; }
    }
}