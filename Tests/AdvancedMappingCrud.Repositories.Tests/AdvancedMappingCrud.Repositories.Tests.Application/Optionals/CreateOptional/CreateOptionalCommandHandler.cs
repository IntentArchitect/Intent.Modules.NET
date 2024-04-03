using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Optionals.CreateOptional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOptionalCommandHandler : IRequestHandler<CreateOptionalCommand, Guid>
    {
        private readonly IOptionalRepository _optionalRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOptionalCommandHandler(IOptionalRepository optionalRepository)
        {
            _optionalRepository = optionalRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOptionalCommand request, CancellationToken cancellationToken)
        {
            var optional = new Optional
            {
                Name = request.Name
            };

            _optionalRepository.Add(optional);
            await _optionalRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return optional.Id;
        }
    }
}