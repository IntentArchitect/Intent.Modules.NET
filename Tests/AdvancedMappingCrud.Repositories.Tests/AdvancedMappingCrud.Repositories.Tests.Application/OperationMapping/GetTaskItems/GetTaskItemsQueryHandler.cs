using System;
using System.Collections.Generic;
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

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.GetTaskItems
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTaskItemsQueryHandler : IRequestHandler<GetTaskItemsQuery, List<TaskItemDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetTaskItemsQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<TaskItemDto>> Handle(GetTaskItemsQuery request, CancellationToken cancellationToken)
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

            var taskItems = taskList.TaskItems;
            return taskItems.MapToTaskItemDtoList(_mapper);
        }
    }
}