using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MassTransit.Messages.Shared.Users;
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
    [IntentManaged(Mode.Fully)]
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UsersService(IUserRepository userRepository, IEventBus eventBus)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.UserName,
                Preferences = dto.Preferences
                    .Select(p => new Preference
                    {
                        Key = p.Key,
                        Value = p.Value
                    })
                    .ToList()
            };

            _userRepository.Add(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(new UserCreatedEvent
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Type = user.Type,
                Preferences = user.Preferences
                    .Select(p => new global::MassTransit.Messages.Shared.Users.PreferenceDto
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Value = p.Value,
                        UserId = p.UserId
                    })
                    .ToList()
            });
            return user.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindUserById (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindUsers (UsersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateUser(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            user.Email = dto.Email;
            user.UserName = dto.UserName;
            user.Preferences = UpdateHelper.CreateOrUpdateCollection(user.Preferences, dto.Preferences, (e, d) => e.Id == d.Id, CreateOrUpdatePreference);
            _eventBus.Publish(new UserUpdatedEvent
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Type = user.Type,
                Preferences = user.Preferences
                    .Select(p => new global::MassTransit.Messages.Shared.Users.PreferenceDto
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Value = p.Value,
                        UserId = p.UserId
                    })
                    .ToList()
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteUser(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            _userRepository.Remove(user);
            _eventBus.Publish(new UserDeletedEvent
            {
                Id = user.Id
            });
        }

        [IntentManaged(Mode.Fully)]
        private static Preference CreateOrUpdatePreference(Preference? entity, Users.PreferenceDto dto)
        {

            entity ??= new Preference();
            entity.Id = dto.Id;
            entity.Key = dto.Key;
            entity.Value = dto.Value;
            return entity;
        }
    }
}