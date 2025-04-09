using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.People;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces
{
    public interface IPersonService
    {
        Task<List<GetPersonsBySurnamePersonDCDto>> GetPersonsBySurname(string surname, CancellationToken cancellationToken = default);
    }
}