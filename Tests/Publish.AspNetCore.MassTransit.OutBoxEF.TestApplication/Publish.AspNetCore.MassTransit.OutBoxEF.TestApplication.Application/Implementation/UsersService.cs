using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Interfaces;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Common;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Common.Exceptions;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UsersService(IUserRepository userRepository, IMapper mapper, IEventBus eventBus)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newUser = new User
            {
                Email = dto.Email,
                UserName = dto.UserName,
                Preferences = dto.Preferences.Select(CreatePreference).ToList(),
            };
            _userRepository.Add(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newUser.MapToUserCreatedEvent());
            return newUser.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            return element.MapToUserDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default)
        {
            var elements = await _userRepository.FindAllAsync(cancellationToken);
            return elements.MapToUserDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateUser(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            existingUser.Email = dto.Email;
            existingUser.UserName = dto.UserName;
            existingUser.Preferences = UpdateHelper.CreateOrUpdateCollection(existingUser.Preferences, dto.Preferences, (e, d) => e.Id == d.Id, CreateOrUpdatePreference);
            _eventBus.Publish(existingUser.MapToUserUpdatedEvent());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteUser(Guid id, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.FindByIdAsync(id, cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException($"Could not find User {id}");
            }
            _userRepository.Remove(existingUser);
            _eventBus.Publish(existingUser.MapToUserDeletedEvent());
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private Preference CreatePreference(Users.PreferenceDto dto)
        {
            return new Preference
            {
                Key = dto.Key,
                Value = dto.Value,
                UserId = dto.UserId,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static Preference CreateOrUpdatePreference(Preference? entity, Users.PreferenceDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new Preference();
            entity.Key = dto.Key;
            entity.Value = dto.Value;
            entity.UserId = dto.UserId;

            return entity;
        }
    }
}