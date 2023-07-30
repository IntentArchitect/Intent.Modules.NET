using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.Operation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationCommandHandler : IRequestHandler<OperationCommand>
    {
        private readonly IOneToOneSourceRepository _oneToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public OperationCommandHandler(IOneToOneSourceRepository oneToOneSourceRepository)
        {
            _oneToOneSourceRepository = oneToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationCommand request, CancellationToken cancellationToken)
        {
            var existingOneToOneSource = await _oneToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOneToOneSource is null)
            {
                throw new NotFoundException($"Could not find OneToOneSource '{request.Id}'");
            }

#warning No supported convention for populating "oneToOneDest" parameter
            existingOneToOneSource.Operation(request.Attribute, oneToOneDest: default);
        }
    }
}