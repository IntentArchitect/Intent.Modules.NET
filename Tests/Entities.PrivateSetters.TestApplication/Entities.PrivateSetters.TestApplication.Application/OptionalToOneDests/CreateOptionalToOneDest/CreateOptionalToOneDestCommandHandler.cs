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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.CreateOptionalToOneDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOptionalToOneDestCommandHandler : IRequestHandler<CreateOptionalToOneDestCommand, Guid>
    {
        private readonly IOptionalToOneDestRepository _optionalToOneDestRepository;
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToOneDestCommandHandler(IOptionalToOneDestRepository optionalToOneDestRepository)
        {
            _optionalToOneDestRepository = optionalToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateOptionalToOneDestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}