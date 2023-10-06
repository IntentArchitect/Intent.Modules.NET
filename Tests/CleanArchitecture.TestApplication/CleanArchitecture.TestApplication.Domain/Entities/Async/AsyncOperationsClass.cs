using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.Async
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AsyncOperationsClass : IHasDomainEvent
    {
        public AsyncOperationsClass()
        {
        }

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