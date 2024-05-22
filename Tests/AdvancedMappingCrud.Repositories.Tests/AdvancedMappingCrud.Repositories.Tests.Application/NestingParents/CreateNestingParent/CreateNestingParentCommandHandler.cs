using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.CreateNestingParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateNestingParentCommandHandler : IRequestHandler<CreateNestingParentCommand, Guid>
    {
        private readonly INestingParentRepository _nestingParentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateNestingParentCommandHandler(INestingParentRepository nestingParentRepository)
        {
            _nestingParentRepository = nestingParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateNestingParentCommand request, CancellationToken cancellationToken)
        {
            var nestingParent = new NestingParent
            {
                Name = request.Name,
                NestingChildren = request.NestingChildren
                    .Select(nc => new NestingChild
                    {
                        Description = nc.Description,
                        NestingChildChild = new NestingChildChild
                        {
                            Name = nc.ChildChild.Name
                        }
                    })
                    .ToList()
            };

            _nestingParentRepository.Add(nestingParent);
            await _nestingParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return nestingParent.Id;
        }
    }
}