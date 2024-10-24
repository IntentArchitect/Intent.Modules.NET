using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Interfaces
{
    public interface IUsersService : IDisposable
    {
        Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default);
        Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default);
        Task UpdateUser(Guid id, UserUpdateDto dto, List<NewDTO> addresses, CancellationToken cancellationToken = default);
        Task DeleteUser(Guid id, CancellationToken cancellationToken = default);
    }
}