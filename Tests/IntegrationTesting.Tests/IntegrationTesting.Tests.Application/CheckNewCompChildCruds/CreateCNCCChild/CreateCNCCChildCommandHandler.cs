using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCNCCChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCNCCChildCommandHandler : IRequestHandler<CreateCNCCChildCommand, Guid>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCNCCChildCommandHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCNCCChildCommand request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.CheckNewCompChildCrudId, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.CheckNewCompChildCrudId}'");
            }
            var cNCCChild = new CNCCChild
            {
                Description = request.Description,
                CheckNewCompChildCrudId = request.CheckNewCompChildCrudId
            };

            checkNewCompChildCrud.CNCCChildren.Add(cNCCChild);
            await _checkNewCompChildCrudRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return cNCCChild.Id;
        }
    }
}