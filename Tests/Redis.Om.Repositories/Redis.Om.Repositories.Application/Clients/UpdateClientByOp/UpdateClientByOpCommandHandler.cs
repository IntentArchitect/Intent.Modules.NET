using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Common.Exceptions;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.UpdateClientByOp
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClientByOpCommandHandler : IRequestHandler<UpdateClientByOpCommand>
    {
        private readonly IClientRepository _clientRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateClientByOpCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateClientByOpCommand request, CancellationToken cancellationToken)
        {
            var entity = await _clientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Client '{request.Id}'");
            }

            entity.Update(request.Type, request.Name);

            _clientRepository.Update(entity);
        }
    }
}