using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationOneToManySourceCommandHandler : IRequestHandler<OperationOneToManySourceCommand>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public OperationOneToManySourceCommandHandler(IOneToManySourceRepository oneToManySourceRepository)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationOneToManySourceCommand request, CancellationToken cancellationToken)
        {
            var existingOneToManySource = await _oneToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOneToManySource is null)
            {
                throw new NotFoundException($"Could not find OneToManySource '{request.Id}'");
            }

#warning No supported convention for populating "owneds" parameter
            existingOneToManySource.Operation(request.Attribute, owneds: default);
        }
    }
}