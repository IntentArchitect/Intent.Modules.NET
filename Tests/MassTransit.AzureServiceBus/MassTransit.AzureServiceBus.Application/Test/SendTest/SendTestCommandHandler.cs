using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Application.IntegrationServices;
using MassTransit.AzureServiceBus.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.Test.SendTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SendTestCommandHandler : IRequestHandler<SendTestCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly ICQRSService _cqrsService;

        [IntentManaged(Mode.Merge)]
        public SendTestCommandHandler(IEventBus eventBus, ICQRSService cqrsService)
        {
            _eventBus = eventBus;
            _cqrsService = cqrsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SendTestCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new TestMessageEvent() { Message = request.Message });
            var response = await _cqrsService.CommandDtoReturnAsync(CommandDtoReturn.Create(request.Message), cancellationToken);
        }
    }
}