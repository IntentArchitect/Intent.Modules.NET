using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OperationMapping;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateUserWithTaskItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserWithTaskItemCommandHandler : IRequestHandler<CreateUserWithTaskItemCommand>
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public CreateUserWithTaskItemCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateUserWithTaskItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new User
            {
                UserName = request.UserName
            };
            entity.AddTask(request.ListName, new TaskItem
            {
                Name = request.TaskName,
                SubTasks = request.SubTasks
                    .Select(st => new SubTask
                    {
                        Name = st.Name
                    })
                    .ToList()
            });

            _userRepository.Add(entity);
        }
    }
}