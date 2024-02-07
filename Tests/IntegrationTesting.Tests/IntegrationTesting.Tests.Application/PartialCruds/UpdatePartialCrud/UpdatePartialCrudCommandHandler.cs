using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.UpdatePartialCrud
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePartialCrudCommandHandler : IRequestHandler<UpdatePartialCrudCommand>
    {
        private readonly IPartialCrudRepository _partialCrudRepository;

        [IntentManaged(Mode.Merge)]
        public UpdatePartialCrudCommandHandler(IPartialCrudRepository partialCrudRepository)
        {
            _partialCrudRepository = partialCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdatePartialCrudCommand request, CancellationToken cancellationToken)
        {
            var partialCrud = await _partialCrudRepository.FindByIdAsync(request.Id, cancellationToken);
            if (partialCrud is null)
            {
                throw new NotFoundException($"Could not find PartialCrud '{request.Id}'");
            }

            partialCrud.Name = request.Name;
        }
    }
}