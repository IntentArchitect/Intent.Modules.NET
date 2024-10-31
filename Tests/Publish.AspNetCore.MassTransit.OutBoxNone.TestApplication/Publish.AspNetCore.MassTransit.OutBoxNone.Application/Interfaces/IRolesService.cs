using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Interfaces
{
    public interface IRolesService
    {
        Task<Guid> CreateRole(RoleCreateDto dto, CancellationToken cancellationToken = default);
        Task<RoleDto> FindRoleById(Guid id, CancellationToken cancellationToken = default);
        Task<List<RoleDto>> FindRoles(CancellationToken cancellationToken = default);
        Task UpdateRole(Guid id, RoleUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteRole(Guid id, CancellationToken cancellationToken = default);
    }
}