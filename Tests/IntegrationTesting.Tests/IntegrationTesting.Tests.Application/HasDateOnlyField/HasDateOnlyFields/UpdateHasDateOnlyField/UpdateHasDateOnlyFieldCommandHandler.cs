using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.UpdateHasDateOnlyField
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateHasDateOnlyFieldCommandHandler : IRequestHandler<UpdateHasDateOnlyFieldCommand>
    {
        private readonly IHasDateOnlyFieldRepository _hasDateOnlyFieldRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateHasDateOnlyFieldCommandHandler(IHasDateOnlyFieldRepository hasDateOnlyFieldRepository)
        {
            _hasDateOnlyFieldRepository = hasDateOnlyFieldRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateHasDateOnlyFieldCommand request, CancellationToken cancellationToken)
        {
            var hasDateOnlyField = await _hasDateOnlyFieldRepository.FindByIdAsync(request.Id, cancellationToken);
            if (hasDateOnlyField is null)
            {
                throw new NotFoundException($"Could not find HasDateOnlyField '{request.Id}'");
            }

            hasDateOnlyField.MyDate = request.MyDate;
        }
    }
}