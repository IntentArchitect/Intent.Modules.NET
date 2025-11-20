using System;
using System.Threading;
using System.Threading.Tasks;
using CompositePublishTest.Application.Common.Eventing;
using CompositePublishTest.Domain.Entities;
using CompositePublishTest.Domain.Repositories;
using CompositePublishTest.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CompositePublishTest.Application.Clients.CreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public CreateClientCommandHandler(IClientRepository clientRepository, IMessageBus messageBus)
        {
            _clientRepository = clientRepository;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = new Client
            {
                Name = request.Name,
                Location = request.Location,
                Description = request.Description
            };

            _clientRepository.Add(client);
            await _clientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _messageBus.Publish(new ClientCreatedEvent
            {
                Name = request.Name,
                Location = request.Location,
                Description = request.Description
            });
            return client.Id;
        }
    }
}