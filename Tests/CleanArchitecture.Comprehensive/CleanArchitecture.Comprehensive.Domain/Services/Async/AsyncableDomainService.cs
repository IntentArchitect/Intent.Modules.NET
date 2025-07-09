using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Services.Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AsyncableDomainService : IAsyncableDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AsyncableDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Mutation()
        {
            // TODO: Implement Mutation (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Mutation(string param)
        {
            // TODO: Implement Mutation (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task MutationAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement MutationAsync (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task MutationAsync(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement MutationAsync (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public string Query()
        {
            // TODO: Implement Query (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public string Query(string param)
        {
            // TODO: Implement Query (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> QueryAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement QueryAsync (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> QueryAsync(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement QueryAsync (AsyncableDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}