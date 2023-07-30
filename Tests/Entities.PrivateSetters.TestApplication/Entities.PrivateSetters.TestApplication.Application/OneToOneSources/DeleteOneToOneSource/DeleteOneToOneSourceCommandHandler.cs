using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.DeleteOneToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOneToOneSourceCommandHandler : IRequestHandler<DeleteOneToOneSourceCommand>
    {
        private readonly IOneToOneSourceRepository _oneToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOneToOneSourceCommandHandler(IOneToOneSourceRepository oneToOneSourceRepository)
        {
            _oneToOneSourceRepository = oneToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOneToOneSourceCommand request, CancellationToken cancellationToken)
        {
            var existingOneToOneSource = await _oneToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOneToOneSource is null)
            {
                throw new NotFoundException($"Could not find OneToOneSource '{request.Id}'");
            }

            _oneToOneSourceRepository.Remove(existingOneToOneSource);
        }
    }
}