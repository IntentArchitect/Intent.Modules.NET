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
        public UsersService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Create(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newUser = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Roles = dto.Roles.Select(CreateRole).ToList(),
            };
            _userRepository.Add(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newUser.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            return element.MapToUserDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindAll(CancellationToken cancellationToken = default)
        {
            var elements = await _userRepository.FindAllAsync(cancellationToken);
            return elements.MapToUserDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Put(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            existingUser.Email = dto.Email;
            existingUser.Username = dto.Username;
            existingUser.Roles = UpdateHelper.CreateOrUpdateCollection(existingUser.Roles, dto.Roles, (e, d) => e.Id == d.Id, CreateOrUpdateRole);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            _userRepository.Remove(existingUser);
            return existingUser.MapToUserDto(_mapper);
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private Role CreateRole(CreateUserRoleDto dto)
        {
            return new Role
            {
                Name = dto.Name,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static Role CreateOrUpdateRole(Role? entity, UpdateUserRoleDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new Role();
            entity.UserId = dto.UserId;
            entity.Name = dto.Name;

            return entity;
        }
    }
}