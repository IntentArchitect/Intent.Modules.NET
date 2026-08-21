using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolverine.EntityFrameworkCore;
using Wolverine.Publish.RabbitMQ.Infrastructure.Persistence;
using ContractsMessageBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;

namespace Wolverine.Publish.RabbitMQ.Infrastructure.Eventing
{
    public class WolverineMessageBus : ContractsMessageBus
    {
        private readonly IDbContextOutbox<ApplicationDbContext> _outbox;
        private readonly List<Func<IDbContextOutbox<ApplicationDbContext>, Task>> _pendingActions = new();

        public WolverineMessageBus(IDbContextOutbox<ApplicationDbContext> outbox)
        {
            _outbox = outbox;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(outbox => outbox.PublishAsync(message).AsTask());
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _pendingActions.Add(outbox => outbox.SendAsync(message).AsTask());
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_pendingActions.Count == 0)
            {
                return;
            }

            var actionsToFlush = new List<Func<IDbContextOutbox<ApplicationDbContext>, Task>>(_pendingActions);
            _pendingActions.Clear();

            foreach (var action in actionsToFlush)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await action(_outbox);
            }

            await _outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
        }
    }
}
