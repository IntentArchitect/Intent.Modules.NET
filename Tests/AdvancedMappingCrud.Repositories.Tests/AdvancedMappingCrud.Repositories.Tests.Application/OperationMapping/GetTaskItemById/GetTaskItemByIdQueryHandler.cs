using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Users;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OperationMapping;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItemById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTaskItemByIdQueryHandler : IRequestHandler<GetTaskItemByIdQuery, TaskItemDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetTaskItemByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TaskItemDto> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{request.UserId}'");
            }

            var taskList = user.TaskLists.SingleOrDefault(x => x.Id == request.TaskListId);
            if (taskList is null)
            {
                throw new NotFoundException($"Could not find TaskList '{request.TaskListId}'");
            }

            var taskItem = taskList.TaskItems.FirstOrDefault(x => x.Id == request.Id);
            if (taskItem is null)
            {
                throw new NotFoundException($"Could not find TaskItem '{request.Id}'");
            }
            return taskItem.MapToTaskItemDto(_mapper);
        }
    }
}