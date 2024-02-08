using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.UpdateBadSignatures
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBadSignaturesCommandHandler : IRequestHandler<UpdateBadSignaturesCommand>
    {
        private readonly IBadSignaturesRepository _badSignaturesRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateBadSignaturesCommandHandler(IBadSignaturesRepository badSignaturesRepository)
        {
            _badSignaturesRepository = badSignaturesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBadSignaturesCommand request, CancellationToken cancellationToken)
        {
            var badSignatures = await _badSignaturesRepository.FindByIdAsync(request.Id, cancellationToken);
            if (badSignatures is null)
            {
                throw new NotFoundException($"Could not find BadSignatures '{request.Id}'");
            }

            badSignatures.Name = request.Name;
        }
    }
}