using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests.DeleteManyToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteManyToManyDestCommandHandler : IRequestHandler<DeleteManyToManyDestCommand>
    {
        private readonly IManyToManyDestRepository _manyToManyDestRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteManyToManyDestCommandHandler(IManyToManyDestRepository manyToManyDestRepository)
        {
            _manyToManyDestRepository = manyToManyDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteManyToManyDestCommand request, CancellationToken cancellationToken)
        {
            var existingManyToManyDest = await _manyToManyDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToManyDest is null)
            {
                throw new NotFoundException($"Could not find ManyToManyDest '{request.Id}'");
            }

            _manyToManyDestRepository.Remove(existingManyToManyDest);
        }
    }
}