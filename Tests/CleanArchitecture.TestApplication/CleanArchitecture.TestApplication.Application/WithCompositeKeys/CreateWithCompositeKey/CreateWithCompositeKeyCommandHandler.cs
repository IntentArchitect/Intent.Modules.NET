using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.WithCompositeKeys.CreateWithCompositeKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateWithCompositeKeyCommandHandler : IRequestHandler<CreateWithCompositeKeyCommand, Guid>
    {
        private readonly IWithCompositeKeyRepository _withCompositeKeyRepository;

        [IntentManaged(Mode.Merge)]
        public CreateWithCompositeKeyCommandHandler(IWithCompositeKeyRepository withCompositeKeyRepository)
        {
            _withCompositeKeyRepository = withCompositeKeyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateWithCompositeKeyCommand request, CancellationToken cancellationToken)
        {
            var newWithCompositeKey = new WithCompositeKey
            {
                Name = request.Name,
            };

            _withCompositeKeyRepository.Add(newWithCompositeKey);
            await _withCompositeKeyRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newWithCompositeKey.Key1Id;
        }
    }
}