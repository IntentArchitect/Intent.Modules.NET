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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.CreateManyToOneDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateManyToOneDestCommandHandler : IRequestHandler<CreateManyToOneDestCommand, Guid>
    {
        private readonly IManyToOneDestRepository _manyToOneDestRepository;
        [IntentManaged(Mode.Merge)]
        public CreateManyToOneDestCommandHandler(IManyToOneDestRepository manyToOneDestRepository)
        {
            _manyToOneDestRepository = manyToOneDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateManyToOneDestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}