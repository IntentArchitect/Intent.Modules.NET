using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.OperationMapping
{
    public interface IUserService
    {
        Task CreateUserWithTaskItemService(string userName, string listName, string taskName, List<CreateUserWithTaskItemServiceSubTasksDto> subTasks, CancellationToken cancellationToken = default);
        Task CreateUserWithTaskItemContractService(string userName, string listName, string taskName, List<CreateUserWithTaskItemContractServiceSubTasksDto> subTasks, CancellationToken cancellationToken = default);
    }
}