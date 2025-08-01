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

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.CreateOneToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneToOneSourceCommandHandler : IRequestHandler<CreateOneToOneSourceCommand, Guid>
    {
        private readonly IOneToOneSourceRepository _oneToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOneToOneSourceCommandHandler(IOneToOneSourceRepository oneToOneSourceRepository)
        {
            _oneToOneSourceRepository = oneToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOneToOneSourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "oneToOneDest" parameter
            var newOneToOneSource = new OneToOneSource(request.Attribute, oneToOneDest: default);

            _oneToOneSourceRepository.Add(newOneToOneSource);
            await _oneToOneSourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newOneToOneSource.Id;
        }
    }
}