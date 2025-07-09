using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ExtensiveDomainService : IExtensiveDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public ExtensiveDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void PerformEntityA(ConcreteEntityA entity)
        {
            // TODO: Implement PerformEntityA (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task PerformEntityAAsync(ConcreteEntityA entity, CancellationToken cancellationToken = default)
        {
            // TODO: Implement PerformEntityAAsync (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void PerformEntityB(ConcreteEntityB entity)
        {
            // TODO: Implement PerformEntityB (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task PerformEntityBAsync(ConcreteEntityB entity, CancellationToken cancellationToken = default)
        {
            // TODO: Implement PerformEntityBAsync (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void PerformPassthrough(PassthroughObj obj)
        {
            // TODO: Implement PerformPassthrough (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task PerformPassthroughAsync(PassthroughObj obj, CancellationToken cancellationToken = default)
        {
            // TODO: Implement PerformPassthroughAsync (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void PerformValueObj(SimpleVO vo)
        {
            // TODO: Implement PerformValueObj (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task PerformValueObjAsync(SimpleVO vo, CancellationToken cancellationToken = default)
        {
            // TODO: Implement PerformValueObjAsync (ExtensiveDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}