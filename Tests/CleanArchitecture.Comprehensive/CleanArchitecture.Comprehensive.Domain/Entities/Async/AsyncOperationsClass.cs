using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.Async
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AsyncOperationsClass : IHasDomainEvent
    {

        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public async Task Explicit(CancellationToken cancellationToken = default)
        {
        }

        public async Task<object> ExplicitWithReturn(CancellationToken cancellationToken = default)
        {
            return new object();
        }

        public async Task ImplicitAsync(CancellationToken cancellationToken = default)
        {
        }

        public async Task<object> ImplicitWithReturnAsync(CancellationToken cancellationToken = default)
        {
            return new object();
        }

        public int Operation()
        {
            return 1;
        }
    }
}