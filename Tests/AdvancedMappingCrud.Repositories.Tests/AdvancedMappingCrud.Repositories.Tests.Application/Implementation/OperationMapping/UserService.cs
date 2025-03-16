using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OperationMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation.OperationMapping
{
    [IntentManaged(Mode.Merge)]
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateUserWithTaskItemService(
            string userName,
            string listName,
            string taskName,
            List<CreateUserWithTaskItemServiceSubTasksDto> subTasks,
            CancellationToken cancellationToken = default)
        {
            var entity = new User
            {
                UserName = userName
            };
            entity.AddTask(listName, new TaskItem
            {
                Name = taskName,
                SubTasks = subTasks
                    .Select(st => new SubTask
                    {
                        Name = st.Name
                    })
                    .ToList()
            });

            _userRepository.Add(entity);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateUserWithTaskItemContractService(
            string userName,
            string listName,
            string taskName,
            List<CreateUserWithTaskItemContractServiceSubTasksDto> subTasks,
            CancellationToken cancellationToken = default)
        {
            var entity = new User
            {
                UserName = userName
            };
            entity.AddTask(listName, new TaskItemContract(
                name: taskName,
                subTasks: subTasks
                    .Select(st => new SubTaskItemContract(
                        name: st.Name))
                    .ToList()));

            _userRepository.Add(entity);
        }
    }
}