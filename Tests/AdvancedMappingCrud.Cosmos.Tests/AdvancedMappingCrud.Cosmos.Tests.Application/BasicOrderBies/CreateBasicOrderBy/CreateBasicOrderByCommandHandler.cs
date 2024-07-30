using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.CreateBasicOrderBy
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBasicOrderByCommandHandler : IRequestHandler<CreateBasicOrderByCommand, string>
    {
        private readonly IBasicOrderByRepository _basicOrderByRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBasicOrderByCommandHandler(IBasicOrderByRepository basicOrderByRepository)
        {
            _basicOrderByRepository = basicOrderByRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateBasicOrderByCommand request, CancellationToken cancellationToken)
        {
            var basicOrderBy = new BasicOrderBy
            {
                Name = request.Name,
                Surname = request.Surname
            };

            _basicOrderByRepository.Add(basicOrderBy);
            await _basicOrderByRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return basicOrderBy.Id;
        }
    }
}