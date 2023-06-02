using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Clients.DeleteClient
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
        public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var existingClient = await _clientRepository.FindByIdAsync(request.Id, cancellationToken);
            _clientRepository.Remove(existingClient);
            return Unit.Value;
        }
    }
}