using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.SeparateDatabase.TestApplication.Application.Interfaces;
using Finbuckle.SeparateDatabase.TestApplication.Application.Users;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Entities;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;

        [IntentManaged(Mode.Merge)]
        public UsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> Create(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = new User { Id = Guid.NewGuid(), Email = dto.Email, Username = dto.Username };
            _userRepository.Add(user);
            await Task.CompletedTask;
            return user.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<UserDto> FindById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindById (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<UserDto>> FindAll(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.FindAllAsync(cancellationToken);
            return users.Select(u => new UserDto { Id = u.Id, Email = u.Email, Username = u.Username, Roles = new List<UserRoleDto>() }).ToList();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Put(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Put (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<UserDto> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Delete (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}