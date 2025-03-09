using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace GrpcServer.Domain.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetOrCreateAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken = default);
    }
}