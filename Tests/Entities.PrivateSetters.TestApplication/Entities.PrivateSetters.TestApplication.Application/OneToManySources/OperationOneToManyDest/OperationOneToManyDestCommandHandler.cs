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

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationOneToManyDestCommandHandler : IRequestHandler<OperationOneToManyDestCommand>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public OperationOneToManyDestCommandHandler(IOneToManySourceRepository oneToManySourceRepository)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationOneToManyDestCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _oneToManySourceRepository.FindByIdAsync(request.OneToManySourceid, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(OneToManySource)} of Id '{request.OneToManySourceid}' could not be found");
            }

            var existingOneToManyDest = aggregateRoot.Owneds.FirstOrDefault(p => p.Id == request.Id);
            if (existingOneToManyDest is null)
            {
                throw new NotFoundException($"{nameof(OneToManyDest)} of Id '{request.Id}' could not be found associated with {nameof(OneToManySource)} of Id '{request.OneToManySourceid}'");
            }

            existingOneToManyDest.Operation(request.Attribute);
        }
    }
}