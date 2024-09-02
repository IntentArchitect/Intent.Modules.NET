using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Category>> GetOrCreateCategoriesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
        void DoIt<T>(T it);
        Task ManualAsync(CancellationToken cancellationToken = default);
    }
}