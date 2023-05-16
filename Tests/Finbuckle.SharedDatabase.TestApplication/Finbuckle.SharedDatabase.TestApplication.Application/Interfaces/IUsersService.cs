using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Finbuckle.SharedDatabase.TestApplication.Application.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Interfaces
{

    public interface IUsersService : IDisposable
    {
        Task<Guid> Create(UserCreateDto dto);
        Task<UserDto> FindById(Guid id);
        Task<List<UserDto>> FindAll();
        Task Put(Guid id, UserUpdateDto dto);
        Task<UserDto> Delete(Guid id);

    }
}