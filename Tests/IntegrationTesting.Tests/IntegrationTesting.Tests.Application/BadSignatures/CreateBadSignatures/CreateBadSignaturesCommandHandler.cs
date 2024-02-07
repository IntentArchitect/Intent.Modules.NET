using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.CreateBadSignatures
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBadSignaturesCommandHandler : IRequestHandler<CreateBadSignaturesCommand, Guid>
    {
        private readonly IBadSignaturesRepository _badSignaturesRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBadSignaturesCommandHandler(IBadSignaturesRepository badSignaturesRepository)
        {
            _badSignaturesRepository = badSignaturesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBadSignaturesCommand request, CancellationToken cancellationToken)
        {
            var badSignatures = new Domain.Entities.BadSignatures
            {
                Name = request.Name
            };

            _badSignaturesRepository.Add(badSignatures);
            await _badSignaturesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return badSignatures.Id;
        }
    }
}