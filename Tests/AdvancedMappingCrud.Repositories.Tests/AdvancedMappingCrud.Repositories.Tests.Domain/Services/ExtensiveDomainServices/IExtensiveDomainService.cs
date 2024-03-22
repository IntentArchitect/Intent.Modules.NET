using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices
{
    public interface IExtensiveDomainService
    {
        void PerformEntityA(ConcreteEntityA entity);
        Task PerformEntityAAsync(ConcreteEntityA entity, CancellationToken cancellationToken = default);
        void PerformEntityB(ConcreteEntityB entity);
        Task PerformEntityBAsync(ConcreteEntityB entity, CancellationToken cancellationToken = default);
        void PerformPassthrough(PassthroughObj obj);
        Task PerformPassthroughAsync(PassthroughObj obj, CancellationToken cancellationToken = default);
        void PerformValueObj(SimpleVO vo);
        Task PerformValueObjAsync(SimpleVO vo, CancellationToken cancellationToken = default);
    }
}