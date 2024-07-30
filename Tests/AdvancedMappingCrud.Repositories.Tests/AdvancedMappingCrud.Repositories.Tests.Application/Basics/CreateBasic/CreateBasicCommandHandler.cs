using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.CreateBasic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBasicCommandHandler : IRequestHandler<CreateBasicCommand, Guid>
    {
        private readonly IBasicRepository _basicRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBasicCommandHandler(IBasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBasicCommand request, CancellationToken cancellationToken)
        {
            var basic = new Basic
            {
                Name = request.Name,
                Surname = request.Surname
            };

            _basicRepository.Add(basic);
            await _basicRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return basic.Id;
        }
    }
}