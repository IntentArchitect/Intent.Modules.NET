using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.CreateOneToManySourceOneToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneToManySourceOneToManyDestCommandHandler : IRequestHandler<CreateOneToManySourceOneToManyDestCommand, Guid>
    {
        private readonly IOneToManySourceRepository _oneToManySourceRepository;
        [IntentManaged(Mode.Merge)]
        public CreateOneToManySourceOneToManyDestCommandHandler(IOneToManySourceRepository oneToManySourceRepository)
        {
            _oneToManySourceRepository = oneToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(
            CreateOneToManySourceOneToManyDestCommand request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}