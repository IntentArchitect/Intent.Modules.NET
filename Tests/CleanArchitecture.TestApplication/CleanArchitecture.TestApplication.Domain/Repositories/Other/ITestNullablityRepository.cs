using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.Other;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Repositories.Other
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITestNullablityRepository : IEFRepository<TestNullablity, TestNullablity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TestNullablity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TestNullablity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}