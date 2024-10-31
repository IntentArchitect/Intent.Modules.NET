using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Interfaces
{
    public interface IUsersService
    {
        Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default);
        Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default);
        Task UpdateUser(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteUser(Guid id, CancellationToken cancellationToken = default);
    }
}