using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests.OperationAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationAsyncCommandHandler : IRequestHandler<OperationAsyncCommand>
    {
        private readonly IManyToManyDestRepository _manyToManyDestRepository;

        [IntentManaged(Mode.Merge)]
        public OperationAsyncCommandHandler(IManyToManyDestRepository manyToManyDestRepository)
        {
            _manyToManyDestRepository = manyToManyDestRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(OperationAsyncCommand request, CancellationToken cancellationToken)
        {
            var existingManyToManyDest = await _manyToManyDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingManyToManyDest is null)
            {
                throw new NotFoundException($"Could not find ManyToManyDest '{request.Id}'");
            }

            await existingManyToManyDest.OperationAsync(request.Attribute, cancellationToken);
        }
    }
}