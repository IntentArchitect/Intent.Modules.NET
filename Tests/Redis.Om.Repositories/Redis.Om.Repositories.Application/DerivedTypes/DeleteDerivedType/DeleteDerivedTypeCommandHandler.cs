using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Common.Exceptions;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.DeleteDerivedType
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDerivedTypeCommandHandler : IRequestHandler<DeleteDerivedTypeCommand>
    {
        private readonly IDerivedTypeRepository _derivedTypeRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDerivedTypeCommandHandler(IDerivedTypeRepository derivedTypeRepository)
        {
            _derivedTypeRepository = derivedTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDerivedTypeCommand request, CancellationToken cancellationToken)
        {
            var derivedType = await _derivedTypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (derivedType is null)
            {
                throw new NotFoundException($"Could not find DerivedType '{request.Id}'");
            }

            _derivedTypeRepository.Remove(derivedType);
        }
    }
}