using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories.Mapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassWithVORepository : IEFRepository<ClassWithVO, ClassWithVO>
    {
        [IntentManaged(Mode.Fully)]
        Task<ClassWithVO?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassWithVO>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}