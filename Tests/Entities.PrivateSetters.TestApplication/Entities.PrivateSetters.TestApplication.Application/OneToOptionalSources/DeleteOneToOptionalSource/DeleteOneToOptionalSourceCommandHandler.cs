using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.DeleteOneToOptionalSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOneToOptionalSourceCommandHandler : IRequestHandler<DeleteOneToOptionalSourceCommand>
    {
        private readonly IOneToOptionalSourceRepository _oneToOptionalSourceRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOneToOptionalSourceCommandHandler(IOneToOptionalSourceRepository oneToOptionalSourceRepository)
        {
            _oneToOptionalSourceRepository = oneToOptionalSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteOneToOptionalSourceCommand request, CancellationToken cancellationToken)
        {
            var existingOneToOptionalSource = await _oneToOptionalSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOneToOptionalSource is null)
            {
                throw new NotFoundException($"Could not find OneToOptionalSource '{request.Id}'");
            }

            _oneToOptionalSourceRepository.Remove(existingOneToOptionalSource);
        }
    }
}