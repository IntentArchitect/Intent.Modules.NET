using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy.CreateChild5
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChild5CommandHandler : IRequestHandler<CreateChild5Command, Guid>
    {
        private readonly IFamilyComplexRepository _familyComplexRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChild5CommandHandler(IFamilyComplexRepository familyComplexRepository)
        {
            _familyComplexRepository = familyComplexRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChild5Command request, CancellationToken cancellationToken)
        {
            var entity = new FamilyComplex(
                childName: request.Dto.ChildName,
                parentId: request.Dto.ParentId,
                parentName: request.Dto.ParentName,
                grandParentId: request.Dto.GrandparentId,
                greatGrandParentId: request.Dto.GreatGrandParentId,
                greatGrandParentName: request.Dto.GreatGrandParentName,
                auntId: request.Dto.Aunt.AuntId,
                auntName: request.Dto.Aunt.AuntName);

            _familyComplexRepository.Add(entity);
            await _familyComplexRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}