using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.DeleteDerivedOfT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDerivedOfTCommandHandler : IRequestHandler<DeleteDerivedOfTCommand>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDerivedOfTCommandHandler(IDerivedOfTRepository derivedOfTRepository)
        {
            _derivedOfTRepository = derivedOfTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT '{request.Id}' ");
            }
            _derivedOfTRepository.Remove(existingDerivedOfT);
            return Unit.Value;
        }
    }
}