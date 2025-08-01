using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.CreateOptionalToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOptionalToOneSourceCommandHandler : IRequestHandler<CreateOptionalToOneSourceCommand, Guid>
    {
        private readonly IOptionalToOneSourceRepository _optionalToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOptionalToOneSourceCommandHandler(IOptionalToOneSourceRepository optionalToOneSourceRepository)
        {
            _optionalToOneSourceRepository = optionalToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOptionalToOneSourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "optionalToOneDest" parameter
            var newOptionalToOneSource = new OptionalToOneSource(request.Attribute, optionalToOneDest: default);

            _optionalToOneSourceRepository.Add(newOptionalToOneSource);
            await _optionalToOneSourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newOptionalToOneSource.Id;
        }
    }
}