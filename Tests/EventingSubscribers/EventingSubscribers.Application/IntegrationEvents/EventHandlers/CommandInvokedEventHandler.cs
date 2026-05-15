using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Application.Interfaces;
using EventingSubscribers.Application.ProcessAccountUpgrade;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandInvokedEventHandler : IIntegrationEventHandler<CommandInvokedEvent>
    {
        private readonly ISender _mediator;
        private readonly IAccountService _accountService;
        [IntentManaged(Mode.Merge)]
        public CommandInvokedEventHandler(ISender mediator, IAccountService accountService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _accountService = accountService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(CommandInvokedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new ProcessAccountUpgradeCommand();
            await _mediator.Send(command, cancellationToken);
            await _accountService.ProcessUpgradeAsync(cancellationToken);
        }
    }
}