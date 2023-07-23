using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Deriveds.DeleteDerived
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDerivedCommandHandler : IRequestHandler<DeleteDerivedCommand>
    {
        private readonly IDerivedRepository _derivedRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDerivedCommandHandler(IDerivedRepository derivedRepository)
        {
            _derivedRepository = derivedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteDerivedCommand request, CancellationToken cancellationToken)
        {
            var existingDerived = await _derivedRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingDerived is null)
            {
                throw new NotFoundException($"Could not find Derived '{request.Id}' ");
            }
            _derivedRepository.Remove(existingDerived);
            return Unit.Value;
        }
    }
}