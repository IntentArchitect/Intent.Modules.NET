using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Users;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItems
{
    public class GetTaskItemsQuery : IRequest<List<TaskItemDto>>, IQuery
    {
        public GetTaskItemsQuery(Guid userId, Guid taskListId)
        {
            UserId = userId;
            TaskListId = taskListId;
        }

        public Guid UserId { get; set; }
        public Guid TaskListId { get; set; }
    }
}