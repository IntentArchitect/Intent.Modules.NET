using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AsyncOperationsClass : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public Task Explicit(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }

        public Task<object> ExplicitWithReturn(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }

        public Task ImplicitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }

        public Task<object> ImplicitWithReturnAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }

        public int Operation()
        {
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}