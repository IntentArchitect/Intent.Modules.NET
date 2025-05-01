using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OperationMapping;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateTaskItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTaskItemCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
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

            var taskItem = new TaskItem
            {
                Name = request.Name,
                TaskListId = request.TaskListId
            };

            taskList.TaskItems.Add(taskItem);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return taskItem.Id;
        }
    }
}