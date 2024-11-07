using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChild2CommandHandler : IRequestHandler<CreateChild2Command, Guid>
    {
        private readonly IChildParentExcludedRepository _childParentExcludedRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChild2CommandHandler(IChildParentExcludedRepository childParentExcludedRepository)
        {
            _childParentExcludedRepository = childParentExcludedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChild2Command request, CancellationToken cancellationToken)
        {
            var entity = new ChildParentExcluded(
                childName: request.Dto.ChildName,
                parentAge: request.Dto.ParentAge);

            _childParentExcludedRepository.Add(entity);
            await _childParentExcludedRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}