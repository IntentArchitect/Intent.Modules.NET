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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.CreateOptionalToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOptionalToManySourceCommandHandler : IRequestHandler<CreateOptionalToManySourceCommand, Guid>
    {
        private readonly IOptionalToManySourceRepository _optionalToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOptionalToManySourceCommandHandler(IOptionalToManySourceRepository optionalToManySourceRepository)
        {
            _optionalToManySourceRepository = optionalToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOptionalToManySourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "optionalOneToManyDests" parameter
            var newOptionalToManySource = new OptionalToManySource(request.Attribute, optionalOneToManyDests: default);

            _optionalToManySourceRepository.Add(newOptionalToManySource);
            await _optionalToManySourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newOptionalToManySource.Id;
        }
    }
}