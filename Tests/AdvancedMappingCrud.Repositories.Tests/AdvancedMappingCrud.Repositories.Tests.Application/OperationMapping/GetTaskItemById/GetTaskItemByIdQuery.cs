using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Users;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItemById
{
    public class GetTaskItemByIdQuery : IRequest<TaskItemDto>, IQuery
    {
        public GetTaskItemByIdQuery(Guid userId, Guid taskListId, Guid id)
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