using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy.CreateChild4
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChild4CommandHandler : IRequestHandler<CreateChild4Command, Guid>
    {
        private readonly IFamilyComplexSkippedRepository _familyComplexSkippedRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChild4CommandHandler(IFamilyComplexSkippedRepository familyComplexSkippedRepository)
        {
            _familyComplexSkippedRepository = familyComplexSkippedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChild4Command request, CancellationToken cancellationToken)
        {
            var entity = new FamilyComplexSkipped
            {
                FamilyComplexSkipped = request.Dto.ChildName,
                FamilyComplexSkipped = request.Dto.ParentId,
                FamilyComplexSkipped = request.Dto.GrandparentId,
                FamilyComplexSkipped = request.Dto.GreatGrandparentId,
                FamilyComplexSkipped = request.Dto.GreatGrandparentName
            };

            _familyComplexSkippedRepository.Add(entity);
            await _familyComplexSkippedRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}