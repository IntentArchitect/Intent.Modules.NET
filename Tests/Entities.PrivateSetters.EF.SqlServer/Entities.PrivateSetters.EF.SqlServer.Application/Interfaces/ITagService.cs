using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application.Interfaces
{

    public interface ITagService
    {
        Task Create(CreateTagDto dto, CancellationToken cancellationToken = default);
        Task<List<TagDto>> GetAll(CancellationToken cancellationToken = default);

    }
}