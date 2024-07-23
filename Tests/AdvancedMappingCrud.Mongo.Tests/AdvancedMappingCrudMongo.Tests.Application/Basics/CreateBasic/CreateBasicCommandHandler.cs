using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.CreateBasic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBasicCommandHandler : IRequestHandler<CreateBasicCommand, string>
    {
        private readonly IBasicRepository _basicRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBasicCommandHandler(IBasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateBasicCommand request, CancellationToken cancellationToken)
        {
            var basic = new Basic
            {
                Name = request.Name
            };

            _basicRepository.Add(basic);
            await _basicRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return basic.Id;
        }
    }
}