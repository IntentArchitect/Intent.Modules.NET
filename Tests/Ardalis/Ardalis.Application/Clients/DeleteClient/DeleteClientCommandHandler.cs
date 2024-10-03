using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Common.Exceptions;
using Ardalis.Domain.Repositories;
using Ardalis.Domain.Specifications;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Ardalis.Application.Clients.DeleteClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.FirstOrDefaultAsync(new ClientSpec(request.Id), cancellationToken);
            if (client is null)
            {
                throw new NotFoundException($"Could not find Client '{request.Id}'");
            }

            _clientRepository.Remove(client);
        }
    }
}