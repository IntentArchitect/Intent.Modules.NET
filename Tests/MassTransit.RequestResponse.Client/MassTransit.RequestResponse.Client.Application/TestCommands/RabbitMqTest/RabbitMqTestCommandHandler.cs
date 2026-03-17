using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.IntegrationServices;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.RabbitMqTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RabbitMqTestCommandHandler : IRequestHandler<RabbitMqTestCommand>
    {
        private readonly IRabbitMqCQRSService _service;

        [IntentManaged(Mode.Merge)]
        public RabbitMqTestCommandHandler(IRabbitMqCQRSService service)
        {
            this._service = service;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(RabbitMqTestCommand request, CancellationToken cancellationToken)
        {
            await _service.CommandVoidReturnAsync(new IntegrationServices.Contracts.RabbitMQ.Services.RequestResponse.CQRS.CommandVoidReturn
            {
                Input = request.Message
            }, cancellationToken);
        }
    }
}