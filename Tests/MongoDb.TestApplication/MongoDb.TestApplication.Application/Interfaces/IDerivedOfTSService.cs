using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.DerivedOfTS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IDerivedOfTSService : IDisposable
    {
        Task<string> CreateDerivedOfT(DerivedOfTCreateDto dto, CancellationToken cancellationToken = default);
        Task<DerivedOfTDto> FindDerivedOfTById(string id, CancellationToken cancellationToken = default);
        Task<List<DerivedOfTDto>> FindDerivedOfTS(CancellationToken cancellationToken = default);
        Task UpdateDerivedOfT(string id, DerivedOfTUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteDerivedOfT(string id, CancellationToken cancellationToken = default);
    }
}