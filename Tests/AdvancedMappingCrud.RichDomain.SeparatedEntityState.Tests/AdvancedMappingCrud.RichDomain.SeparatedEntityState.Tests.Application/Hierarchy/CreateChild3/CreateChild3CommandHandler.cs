using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy.CreateChild3
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChild3CommandHandler : IRequestHandler<CreateChild3Command, Guid>
    {
        private readonly IFamilySimpleRepository _familySimpleRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChild3CommandHandler(IFamilySimpleRepository familySimpleRepository)
        {
            _familySimpleRepository = familySimpleRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChild3Command request, CancellationToken cancellationToken)
        {
            var entity = new FamilySimple
            {
                FamilySimple = request.Dto.ChildName,
                FamilySimple = request.Dto.ParentId,
                FamilySimple = request.Dto.ParentName,
                FamilySimple = request.Dto.GrandparentId,
                FamilySimple = request.Dto.GrandparentName
            };

            _familySimpleRepository.Add(entity);
            await _familySimpleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}