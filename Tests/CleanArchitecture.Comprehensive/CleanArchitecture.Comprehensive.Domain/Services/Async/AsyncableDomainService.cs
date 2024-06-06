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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Mutation()
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Query()
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Mutation(string param)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task MutationAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task MutationAsync(string param, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Query(string param)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> QueryAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> QueryAsync(string param, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}