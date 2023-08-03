using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.CreateOneToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneToManySourceCommandHandler : IRequestHandler<CreateOneToManySourceCommand, Guid>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        [IntentManaged(Mode.Merge)]
        public CreateOneToManySourceCommandHandler(IOneToManySourceRepository oneToManySourceRepository)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateOneToManySourceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}