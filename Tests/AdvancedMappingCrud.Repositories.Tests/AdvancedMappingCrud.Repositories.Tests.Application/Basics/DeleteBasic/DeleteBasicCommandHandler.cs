using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.DeleteBasic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteBasicCommandHandler : IRequestHandler<DeleteBasicCommand>
    {
        private readonly IBasicRepository _basicRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteBasicCommandHandler(IBasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteBasicCommand request, CancellationToken cancellationToken)
        {
            var basic = await _basicRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basic is null)
            {
                throw new NotFoundException($"Could not find Basic '{request.Id}'");
            }

            _basicRepository.Remove(basic);
        }
    }
}