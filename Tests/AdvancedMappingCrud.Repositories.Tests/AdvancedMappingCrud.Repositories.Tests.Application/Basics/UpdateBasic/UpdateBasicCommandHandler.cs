using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.UpdateBasic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBasicCommandHandler : IRequestHandler<UpdateBasicCommand>
    {
        private readonly IBasicRepository _basicRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateBasicCommandHandler(IBasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBasicCommand request, CancellationToken cancellationToken)
        {
            var basic = await _basicRepository.FindByIdAsync(request.Id, cancellationToken);
            if (basic is null)
            {
                throw new NotFoundException($"Could not find Basic '{request.Id}'");
            }

            basic.Name = request.Name;
            basic.Surname = request.Surname;
        }
    }
}