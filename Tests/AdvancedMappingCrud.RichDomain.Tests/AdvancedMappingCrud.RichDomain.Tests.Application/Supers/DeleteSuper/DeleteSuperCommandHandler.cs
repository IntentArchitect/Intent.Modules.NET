using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.DeleteSuper
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteSuperCommandHandler : IRequestHandler<DeleteSuperCommand>
    {
        private readonly ISuperRepository _superRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteSuperCommandHandler(ISuperRepository superRepository)
        {
            _superRepository = superRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteSuperCommand request, CancellationToken cancellationToken)
        {
            var super = await _superRepository.FindByIdAsync(request.Id, cancellationToken);
            if (super is null)
            {
                throw new NotFoundException($"Could not find Super '{request.Id}'");
            }

            _superRepository.Remove(super);
        }
    }
}