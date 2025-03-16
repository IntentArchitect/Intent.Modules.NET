using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateUserWithTaskItem
{
    public class CreateUserWithTaskItemCommand : IRequest, ICommand
    {
        public CreateUserWithTaskItemCommand(string userName,
            string listName,
            string taskName,
            List<CreateUserWithTaskItemCommandSubTasksDto> subTasks)
        {
            UserName = userName;
            ListName = listName;
            TaskName = taskName;
            SubTasks = subTasks;
        }

        public string UserName { get; set; }
        public string ListName { get; set; }
        public string TaskName { get; set; }
        public List<CreateUserWithTaskItemCommandSubTasksDto> SubTasks { get; set; }
    }
}