using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.SqlServer.Application.Interfaces;
using Entities.PrivateSetters.EF.SqlServer.Domain.Common.Exceptions;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Entities.PrivateSetters.EF.SqlServer.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AuditedService : IAuditedService
    {
        private readonly IAuditedRepository _auditedRepository;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AuditedService(IAuditedRepository auditedRepository)
        {
            _auditedRepository = auditedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Create(CancellationToken cancellationToken = default)
        {
            var audited = new Audited();
            _auditedRepository.Add(audited);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Update(Guid id, CancellationToken cancellationToken = default)
        {
            var audited = await _auditedRepository.FindByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Not found");

            audited.UpdateAttribute(Guid.NewGuid());
        }

        public void Dispose()
        {
        }
    }
}