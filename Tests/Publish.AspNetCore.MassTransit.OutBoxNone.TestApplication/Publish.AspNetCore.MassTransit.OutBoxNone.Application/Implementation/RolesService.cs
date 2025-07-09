using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MassTransit.Messages.Shared.Roles;
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

        [IntentManaged(Mode.Merge)]
        public RolesService(IRoleRepository roleRepository, IEventBus eventBus, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateRole(RoleCreateDto dto, CancellationToken cancellationToken = default)
        {
            var role = new Role
            {
                Name = dto.Name,
                Priviledges = dto.Priviledges
                    .Select(p => new Priviledge
                    {
                        Name = p.Name
                    })
                    .ToList()
            };

            _roleRepository.Add(role);
            await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(new RoleCreatedEvent
            {
                Id = role.Id,
                Name = role.Name,
                Priviledges = role.Priviledges
                    .Select(p => new global::MassTransit.Messages.Shared.Roles.PriviledgeDto
                    {
                        Id = p.Id,
                        RoleId = p.RoleId,
                        Name = p.Name
                    })
                    .ToList()
            });
            return role.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<RoleDto> FindRoleById(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.FindByIdAsync(id, cancellationToken);
            if (role is null)
            {
                throw new NotFoundException($"Could not find Role '{id}'");
            }
            return role.MapToRoleDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<RoleDto>> FindRoles(CancellationToken cancellationToken = default)
        {
            var roles = await _roleRepository.FindAllAsync(cancellationToken);
            return roles.MapToRoleDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateRole(Guid id, RoleUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.FindByIdAsync(id, cancellationToken);
            if (role is null)
            {
                throw new NotFoundException($"Could not find Role '{id}'");
            }

            role.Name = dto.Name;
            role.Priviledges = UpdateHelper.CreateOrUpdateCollection(role.Priviledges, dto.Priviledges, (e, d) => e.Id == d.Id, CreateOrUpdatePriviledge);
            _eventBus.Publish(new RoleUpdatedEvent
            {
                Id = role.Id,
                Name = role.Name,
                Priviledges = role.Priviledges
                    .Select(p => new global::MassTransit.Messages.Shared.Roles.PriviledgeDto
                    {
                        Id = p.Id,
                        RoleId = p.RoleId,
                        Name = p.Name
                    })
                    .ToList()
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteRole(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.FindByIdAsync(id, cancellationToken);
            if (role is null)
            {
                throw new NotFoundException($"Could not find Role '{id}'");
            }

            _roleRepository.Remove(role);
            _eventBus.Publish(new RoleDeletedEvent
            {
                Id = role.Id,
                Name = role.Name
            });
        }

        [IntentManaged(Mode.Fully)]
        private static Priviledge CreateOrUpdatePriviledge(Priviledge? entity, Roles.PriviledgeDto dto)
        {
            entity ??= new Priviledge();
            entity.Id = dto.Id;
            entity.Name = dto.Name;
            return entity;
        }
    }
}