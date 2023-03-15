using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Users;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IUsersService : IDisposable
    {

        Task<long> Create(UserCreateDto dto);

        Task<UserDto> FindById(long id);

        Task<List<UserDto>> FindAll();

        Task Put(long id, UserUpdateDto dto);

        Task<UserDto> Delete(long id);

    }
}