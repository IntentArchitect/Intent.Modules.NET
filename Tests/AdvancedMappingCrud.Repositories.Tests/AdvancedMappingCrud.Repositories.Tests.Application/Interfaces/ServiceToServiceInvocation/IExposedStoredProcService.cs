using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.ServiceToServiceInvocation
{
    public interface IExposedStoredProcService
    {
        Task<List<GetDataEntryDto>> GetData(CancellationToken cancellationToken = default);
    }
}