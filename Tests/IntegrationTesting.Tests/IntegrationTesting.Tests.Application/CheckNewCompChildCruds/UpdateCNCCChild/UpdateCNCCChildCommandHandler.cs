using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCNCCChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCNCCChildCommandHandler : IRequestHandler<UpdateCNCCChildCommand>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCNCCChildCommandHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCNCCChildCommand request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.CheckNewCompChildCrudId, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.CheckNewCompChildCrudId}'");
            }

            var cNCCChild = checkNewCompChildCrud.CNCCChildren.FirstOrDefault(x => x.Id == request.Id);
            if (cNCCChild is null)
            {
                throw new NotFoundException($"Could not find CNCCChild '{request.Id}'");
            }

            cNCCChild.Description = request.Description;
            cNCCChild.CheckNewCompChildCrudId = request.CheckNewCompChildCrudId;
        }
    }
}