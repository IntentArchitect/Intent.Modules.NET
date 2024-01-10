using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Common.Eventing;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Interfaces;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Common;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Common.Exceptions;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class RolesService : IRolesService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RolesService(IRoleRepository roleRepository, IMapper mapper, IEventBus eventBus)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateRole(RoleCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newRole = new Role
            {
                Name = dto.Name,
                Priviledges = dto.Priviledges.Select(CreatePriviledge).ToList(),
            };
            _roleRepository.Add(newRole);
            await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newRole.MapToRoleCreatedEvent());
            return newRole.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<RoleDto> FindRoleById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _roleRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find Role {id}");
            }
            return element.MapToRoleDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<RoleDto>> FindRoles(CancellationToken cancellationToken = default)
        {
            var elements = await _roleRepository.FindAllAsync(cancellationToken);
            return elements.MapToRoleDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateRole(Guid id, RoleUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingRole = await _roleRepository.FindByIdAsync(id, cancellationToken);

            if (existingRole is null)
            {
                throw new NotFoundException($"Could not find Role {id}");
            }
            existingRole.Name = dto.Name;
            existingRole.Priviledges = UpdateHelper.CreateOrUpdateCollection(existingRole.Priviledges, dto.Priviledges, (e, d) => e.Id == d.Id, CreateOrUpdatePriviledge);
            _eventBus.Publish(existingRole.MapToRoleUpdatedEvent());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteRole(Guid id, CancellationToken cancellationToken = default)
        {
            var existingRole = await _roleRepository.FindByIdAsync(id, cancellationToken);

            if (existingRole is null)
            {
                throw new NotFoundException($"Could not find Role {id}");
            }
            _roleRepository.Remove(existingRole);
            _eventBus.Publish(existingRole.MapToRoleDeletedEvent());
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private Priviledge CreatePriviledge(Roles.PriviledgeDto dto)
        {
            return new Priviledge
            {
                RoleId = dto.RoleId,
                Name = dto.Name,
            };
        }

        [IntentManaged(Mode.Fully)]
        private static Priviledge CreateOrUpdatePriviledge(Priviledge? entity, Roles.PriviledgeDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            entity ??= new Priviledge();
            entity.RoleId = dto.RoleId;
            entity.Name = dto.Name;

            return entity;
        }
    }
}