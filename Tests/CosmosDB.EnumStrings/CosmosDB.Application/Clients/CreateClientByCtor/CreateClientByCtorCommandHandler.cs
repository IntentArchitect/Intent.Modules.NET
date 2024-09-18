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

namespace CosmosDB.Application.Clients.CreateClientByCtor
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClientByCtorCommandHandler : IRequestHandler<CreateClientByCtorCommand, string>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public CreateClientByCtorCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateClientByCtorCommand request, CancellationToken cancellationToken)
        {
            var newClient = new Client(request.Type, request.Name);

            _clientRepository.Add(newClient);
            await _clientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newClient.Identifier;
        }
    }
}