using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients.UpdateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var existingClient = await _clientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingClient is null)
            {
                throw new NotFoundException($"Could not find Client '{request.Id}'");
            }

            existingClient.Name = request.Name;
            existingClient.TagsIds = request.TagsIds.ToList();

            _clientRepository.Update(existingClient);

        }
    }
}