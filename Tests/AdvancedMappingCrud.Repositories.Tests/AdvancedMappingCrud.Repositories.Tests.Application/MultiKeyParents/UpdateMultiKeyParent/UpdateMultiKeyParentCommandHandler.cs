using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.UpdateMultiKeyParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateMultiKeyParentCommandHandler : IRequestHandler<UpdateMultiKeyParentCommand>
    {
        private readonly IMultiKeyParentRepository _multiKeyParentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateMultiKeyParentCommandHandler(IMultiKeyParentRepository multiKeyParentRepository)
        {
            _multiKeyParentRepository = multiKeyParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateMultiKeyParentCommand request, CancellationToken cancellationToken)
        {
            var multiKeyParent = await _multiKeyParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (multiKeyParent is null)
            {
                throw new NotFoundException($"Could not find MultiKeyParent '{request.Id}'");
            }

            multiKeyParent.Name = request.Name;
            multiKeyParent.MultiKeyChildren = UpdateHelper.CreateOrUpdateCollection(multiKeyParent.MultiKeyChildren, request.MultiKeyChildren, (e, d) => e.Id == d.Id && e.RefNo == d.RefNo, CreateOrUpdateMultiKeyChild);
        }

        [IntentManaged(Mode.Fully)]
        private static MultiKeyChild CreateOrUpdateMultiKeyChild(
            MultiKeyChild? entity,
            UpdateMultiKeyParentCommandMultiKeyChildrenDto dto)
        {
            entity ??= new MultiKeyChild();
            entity.Id = dto.Id;
            entity.RefNo = dto.RefNo;
            entity.ChildName = dto.ChildName;
            return entity;
        }
    }
}