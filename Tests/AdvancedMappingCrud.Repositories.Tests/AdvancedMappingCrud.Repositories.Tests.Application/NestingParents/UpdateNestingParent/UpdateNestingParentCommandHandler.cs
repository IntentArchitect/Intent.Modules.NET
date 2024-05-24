using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.UpdateNestingParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNestingParentCommandHandler : IRequestHandler<UpdateNestingParentCommand>
    {
        private readonly INestingParentRepository _nestingParentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateNestingParentCommandHandler(INestingParentRepository nestingParentRepository)
        {
            _nestingParentRepository = nestingParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNestingParentCommand request, CancellationToken cancellationToken)
        {
            var nestingParent = await _nestingParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (nestingParent is null)
            {
                throw new NotFoundException($"Could not find NestingParent '{request.Id}'");
            }

            nestingParent.Name = request.Name;
        }
    }
}