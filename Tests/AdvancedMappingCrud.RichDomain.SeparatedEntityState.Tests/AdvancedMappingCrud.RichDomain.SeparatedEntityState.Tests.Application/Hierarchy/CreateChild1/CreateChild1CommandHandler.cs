using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy.CreateChild1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChild1CommandHandler : IRequestHandler<CreateChild1Command, Guid>
    {
        private readonly IChildSimpleRepository _childSimpleRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChild1CommandHandler(IChildSimpleRepository childSimpleRepository)
        {
            _childSimpleRepository = childSimpleRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChild1Command request, CancellationToken cancellationToken)
        {
            var entity = new ChildSimple(
                childName: request.Dto.ChildName,
                parentName: request.Dto.ParentName);

            _childSimpleRepository.Add(entity);
            await _childSimpleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}