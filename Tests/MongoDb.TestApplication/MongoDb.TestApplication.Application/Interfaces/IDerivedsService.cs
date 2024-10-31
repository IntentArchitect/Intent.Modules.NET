using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Deriveds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{
    public interface IDerivedsService
    {
        Task<string> CreateDerived(DerivedCreateDto dto, CancellationToken cancellationToken = default);
        Task<DerivedDto> FindDerivedById(string id, CancellationToken cancellationToken = default);
        Task<List<DerivedDto>> FindDeriveds(CancellationToken cancellationToken = default);
        Task UpdateDerived(string id, DerivedUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteDerived(string id, CancellationToken cancellationToken = default);
    }
}