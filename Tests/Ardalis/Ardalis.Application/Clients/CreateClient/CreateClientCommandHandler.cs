using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Entities;
using Ardalis.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Ardalis.Application.Clients.CreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public CreateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = new Client(
                name: request.Name);

            _clientRepository.Add(client);
            await _clientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return client.Id;
        }
    }
}