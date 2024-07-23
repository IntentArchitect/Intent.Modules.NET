using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.DeleteBasicOrderBy
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteBasicOrderByCommandHandler : IRequestHandler<DeleteBasicOrderByCommand>
    {
        private readonly IBasicOrderByRepository _basicOrderByRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteBasicOrderByCommandHandler(IBasicOrderByRepository basicOrderByRepository)
        {
            _basicOrderByRepository = basicOrderByRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteBasicOrderByCommand request, CancellationToken cancellationToken)
        {
            var basicOrderBy = await _basicOrderByRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basicOrderBy is null)
            {
                throw new NotFoundException($"Could not find BasicOrderBy '{request.Id}'");
            }

            _basicOrderByRepository.Remove(basicOrderBy);
        }
    }
}