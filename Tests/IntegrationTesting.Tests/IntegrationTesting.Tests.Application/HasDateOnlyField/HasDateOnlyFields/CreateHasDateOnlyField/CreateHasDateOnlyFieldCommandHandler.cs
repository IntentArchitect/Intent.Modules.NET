using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.CreateHasDateOnlyField
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateHasDateOnlyFieldCommandHandler : IRequestHandler<CreateHasDateOnlyFieldCommand, Guid>
    {
        private readonly IHasDateOnlyFieldRepository _hasDateOnlyFieldRepository;

        [IntentManaged(Mode.Merge)]
        public CreateHasDateOnlyFieldCommandHandler(IHasDateOnlyFieldRepository hasDateOnlyFieldRepository)
        {
            _hasDateOnlyFieldRepository = hasDateOnlyFieldRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateHasDateOnlyFieldCommand request, CancellationToken cancellationToken)
        {
            var hasDateOnlyField = new Domain.Entities.HasDateOnlyField
            {
                MyDate = request.MyDate
            };

            _hasDateOnlyFieldRepository.Add(hasDateOnlyField);
            await _hasDateOnlyFieldRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return hasDateOnlyField.Id;
        }
    }
}