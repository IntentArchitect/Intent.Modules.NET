using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.SeparateDatabase.TestApplication.Application.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Interfaces
{

    public interface IUsersService
    {
        Task<Guid> Create(UserCreateDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> FindById(Guid id, CancellationToken cancellationToken = default);
        Task<List<UserDto>> FindAll(CancellationToken cancellationToken = default);
        Task Put(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> Delete(Guid id, CancellationToken cancellationToken = default);

    }
}