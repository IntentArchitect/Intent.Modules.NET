using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Enums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassWithEnumsRepository : IEFRepository<ClassWithEnums, ClassWithEnums>
    {
        [IntentManaged(Mode.Fully)]
        Task<ClassWithEnums?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassWithEnums>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}