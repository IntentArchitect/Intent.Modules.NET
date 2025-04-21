using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.UpdateParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateParentCommandHandler : IRequestHandler<UpdateParentCommand>
    {
        private readonly IParentRepository _parentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandHandler(IParentRepository parentRepository)
        {
            _parentRepository = parentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateParentCommand request, CancellationToken cancellationToken)
        {
            var parent = await _parentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (parent is null)
            {
                throw new NotFoundException($"Could not find Parent '{request.Id}'");
            }

            parent.Name = request.Name;
            parent.Children = UpdateHelper.CreateOrUpdateCollection(parent.Children, request.Children, (e, d) => e.Id == d.Id, CreateOrUpdateChild);
            if (request.ParentDetails != null)
            {
                parent.ParentDetails ??= new ParentDetails();
                parent.ParentDetails.DetailsLine1 = request.ParentDetails.DetailsLine1;
                parent.ParentDetails.DetailsLine2 = request.ParentDetails.DetailsLine2;
                if (request.ParentDetails?.ParentSubDetails != null)
                {
                    parent.ParentDetails.ParentSubDetails ??= new ParentSubDetails();
                    parent.ParentDetails.ParentSubDetails.SubDetailsLine1 = request.ParentDetails.ParentSubDetails.SubDetailsLine1;
                    parent.ParentDetails.ParentSubDetails.SubDetailsLine2 = request.ParentDetails.ParentSubDetails.SubDetailsLine2;
                }
                parent.ParentDetails.ParentDetailsTags = UpdateHelper.CreateOrUpdateCollection(parent.ParentDetails.ParentDetailsTags, request.ParentDetails.ParentDetailsTags, (e, d) => false, CreateOrUpdateParentDetailsTags);
            }

            _parentRepository.Update(parent);
        }

        [IntentManaged(Mode.Fully)]
        private static Child? CreateOrUpdateChild(Child? entity, UpdateParentCommandChildrenDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new Child();
            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.Age = dto.Age;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static ParentDetailsTags? CreateOrUpdateParentDetailsTags(
            ParentDetailsTags? entity,
            UpdateParentCommandParentDetailsTagsDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new ParentDetailsTags();
            entity.TagName = dto.TagName;
            entity.TagValue = dto.TagValue;
            return entity;
        }
    }
}