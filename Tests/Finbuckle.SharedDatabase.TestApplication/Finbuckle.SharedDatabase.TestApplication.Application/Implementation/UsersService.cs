using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.SharedDatabase.TestApplication.Application.Interfaces;
using Finbuckle.SharedDatabase.TestApplication.Application.Users;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UsersService : IUsersService
    {
        [IntentManaged(Mode.Merge)]
        public UsersService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Create(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Create (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<UserDto> FindById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindById (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<UserDto>> FindAll(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindAll (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Put(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Put (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<UserDto> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Delete (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}