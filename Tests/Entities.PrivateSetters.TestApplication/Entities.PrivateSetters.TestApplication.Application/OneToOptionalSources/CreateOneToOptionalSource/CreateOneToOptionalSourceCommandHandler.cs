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

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.CreateOneToOptionalSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneToOptionalSourceCommandHandler : IRequestHandler<CreateOneToOptionalSourceCommand, Guid>
    {
        private readonly IOneToOptionalSourceRepository _oneToOptionalSourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOneToOptionalSourceCommandHandler(IOneToOptionalSourceRepository oneToOptionalSourceRepository)
        {
            _oneToOptionalSourceRepository = oneToOptionalSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOneToOptionalSourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "oneToOptionalDest" parameter
            var newOneToOptionalSource = new OneToOptionalSource(request.Attribute, oneToOptionalDest: default);

            _oneToOptionalSourceRepository.Add(newOneToOptionalSource);
            await _oneToOptionalSourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newOneToOptionalSource.Id;
        }
    }
}