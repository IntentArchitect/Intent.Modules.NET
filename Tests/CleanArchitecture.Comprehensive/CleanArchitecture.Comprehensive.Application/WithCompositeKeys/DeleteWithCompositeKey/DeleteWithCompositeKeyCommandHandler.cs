using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.DeleteWithCompositeKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteWithCompositeKeyCommandHandler : IRequestHandler<DeleteWithCompositeKeyCommand>
    {
        private readonly IWithCompositeKeyRepository _withCompositeKeyRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteWithCompositeKeyCommandHandler(IWithCompositeKeyRepository withCompositeKeyRepository)
        {
            _withCompositeKeyRepository = withCompositeKeyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteWithCompositeKeyCommand request, CancellationToken cancellationToken)
        {
            var existingWithCompositeKey = await _withCompositeKeyRepository.FindByIdAsync((request.Key1Id, request.Key2Id), cancellationToken);
            if (existingWithCompositeKey is null)
            {
                throw new NotFoundException($"Could not find WithCompositeKey '({request.Key1Id}, {request.Key2Id})'");
            }

            _withCompositeKeyRepository.Remove(existingWithCompositeKey);
        }
    }
}