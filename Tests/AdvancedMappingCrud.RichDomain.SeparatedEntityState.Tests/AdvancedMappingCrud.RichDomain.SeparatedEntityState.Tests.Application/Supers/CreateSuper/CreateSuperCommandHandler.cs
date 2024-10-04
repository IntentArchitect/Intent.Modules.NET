using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Supers.CreateSuper
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateSuperCommandHandler : IRequestHandler<CreateSuperCommand, Guid>
    {
        private readonly ISuperRepository _superRepository;

        [IntentManaged(Mode.Merge)]
        public CreateSuperCommandHandler(ISuperRepository superRepository)
        {
            _superRepository = superRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateSuperCommand request, CancellationToken cancellationToken)
        {
            var super = new Super(
                name: request.Name);

            _superRepository.Add(super);
            await _superRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return super.Id;
        }
    }
}