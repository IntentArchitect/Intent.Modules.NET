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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void PerformEntityA(ConcreteEntityA entity)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task PerformEntityAAsync(ConcreteEntityA entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void PerformEntityB(ConcreteEntityB entity)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task PerformEntityBAsync(ConcreteEntityB entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void PerformPassthrough(PassthroughObj obj)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task PerformPassthroughAsync(PassthroughObj obj, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void PerformValueObj(SimpleVO vo)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task PerformValueObjAsync(SimpleVO vo, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}