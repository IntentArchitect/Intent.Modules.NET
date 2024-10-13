using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.AnemicChild;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.UpdateParentWithAnemicChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateParentWithAnemicChildCommandHandler : IRequestHandler<UpdateParentWithAnemicChildCommand>
    {
        private readonly IParentWithAnemicChildRepository _parentWithAnemicChildRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateParentWithAnemicChildCommandHandler(IParentWithAnemicChildRepository parentWithAnemicChildRepository)
        {
            _parentWithAnemicChildRepository = parentWithAnemicChildRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateParentWithAnemicChildCommand request, CancellationToken cancellationToken)
        {
            var parentWithAnemicChild = await _parentWithAnemicChildRepository.FindByIdAsync(request.Id, cancellationToken);
            if (parentWithAnemicChild is null)
            {
                throw new NotFoundException($"Could not find ParentWithAnemicChild '{request.Id}'");
            }

            parentWithAnemicChild.Name = request.Name;
            parentWithAnemicChild.Surname = request.Surname;
        }
    }
}