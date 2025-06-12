using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Finbuckle.SeparateDatabase.TestApplication.Application.Interfaces;
using Finbuckle.SeparateDatabase.TestApplication.Application.Users;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Common;
using Finbuckle.SeparateDatabase.TestApplication.Domain.Common.Exceptions;
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
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UsersService()
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Create(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Create (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindById (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindAll(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindAll (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Put (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Delete (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}