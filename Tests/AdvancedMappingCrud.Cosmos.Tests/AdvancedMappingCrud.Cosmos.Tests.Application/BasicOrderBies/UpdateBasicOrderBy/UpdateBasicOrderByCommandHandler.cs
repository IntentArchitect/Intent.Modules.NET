using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.UpdateBasicOrderBy
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBasicOrderByCommandHandler : IRequestHandler<UpdateBasicOrderByCommand>
    {
        private readonly IBasicOrderByRepository _basicOrderByRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateBasicOrderByCommandHandler(IBasicOrderByRepository basicOrderByRepository)
        {
            _basicOrderByRepository = basicOrderByRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBasicOrderByCommand request, CancellationToken cancellationToken)
        {
            var basicOrderBy = await _basicOrderByRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basicOrderBy is null)
            {
                throw new NotFoundException($"Could not find BasicOrderBy '{request.Id}'");
            }

            basicOrderBy.Name = request.Name;
            basicOrderBy.Surname = request.Surname;

            _basicOrderByRepository.Update(basicOrderBy);
        }
    }
}