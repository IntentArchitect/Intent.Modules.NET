using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Clients.CreateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, string>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public CreateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var newClient = new Client
            {
                Type = request.Type,
                Name = request.Name,
            };

            _clientRepository.Add(newClient);
            await _clientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newClient.Identifier;
        }
    }
}