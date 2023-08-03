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

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.CreateOptionalToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOptionalToManyDestCommandHandler : IRequestHandler<CreateOptionalToManyDestCommand, Guid>
    {
        private readonly IOptionalToManyDestRepository _optionalToManyDestRepository;
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToManyDestCommandHandler(IOptionalToManyDestRepository optionalToManyDestRepository)
        {
            _optionalToManyDestRepository = optionalToManyDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateOptionalToManyDestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}