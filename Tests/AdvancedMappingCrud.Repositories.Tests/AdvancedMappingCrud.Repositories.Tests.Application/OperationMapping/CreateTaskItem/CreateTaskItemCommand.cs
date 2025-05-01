using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateTaskItem
{
    public class CreateTaskItemCommand : IRequest<Guid>, ICommand
    {
        public CreateTaskItemCommand(Guid userId, Guid taskListId, string name)
        {
            UserId = userId;
            TaskListId = taskListId;
            Name = name;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
        public string Name { get; set; }
    }
}