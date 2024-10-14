using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.AnemicChild;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.CreateParentWithAnemicChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateParentWithAnemicChildCommandHandler : IRequestHandler<CreateParentWithAnemicChildCommand, Guid>
    {
        private readonly IParentWithAnemicChildRepository _parentWithAnemicChildRepository;

        [IntentManaged(Mode.Merge)]
        public CreateParentWithAnemicChildCommandHandler(IParentWithAnemicChildRepository parentWithAnemicChildRepository)
        {
            _parentWithAnemicChildRepository = parentWithAnemicChildRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateParentWithAnemicChildCommand request, CancellationToken cancellationToken)
        {
            var parentWithAnemicChild = new ParentWithAnemicChild(
                name: request.Name,
                surname: request.Surname,
                anemicChildren: request.AnemicChildren
                    .Select(c => new AnemicChild
                    {
                        Line1 = c.Line1,
                        Line2 = c.Line2,
                        City = c.City
                    })
                    .ToList());

            _parentWithAnemicChildRepository.Add(parentWithAnemicChild);
            await _parentWithAnemicChildRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return parentWithAnemicChild.Id;
        }
    }
}