using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.CreateMultiKeyParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMultiKeyParentCommandHandler : IRequestHandler<CreateMultiKeyParentCommand, Guid>
    {
        private readonly IMultiKeyParentRepository _multiKeyParentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateMultiKeyParentCommandHandler(IMultiKeyParentRepository multiKeyParentRepository)
        {
            _multiKeyParentRepository = multiKeyParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateMultiKeyParentCommand request, CancellationToken cancellationToken)
        {
            var multiKeyParent = new MultiKeyParent
            {
                Name = request.Name,
                MultiKeyChildren = request.MultiKeyChildren
                    .Select(mkc => new MultiKeyChild
                    {
                        ChildName = mkc.ChildName
                    })
                    .ToList()
            };

            _multiKeyParentRepository.Add(multiKeyParent);
            await _multiKeyParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return multiKeyParent.Id;
        }
    }
}