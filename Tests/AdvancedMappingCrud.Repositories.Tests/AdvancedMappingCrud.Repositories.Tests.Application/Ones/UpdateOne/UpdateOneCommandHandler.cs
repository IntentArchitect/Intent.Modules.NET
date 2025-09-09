using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.NullableNested;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.UpdateOne
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOneCommandHandler : IRequestHandler<UpdateOneCommand>
    {
        private readonly IOneRepository _oneRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateOneCommandHandler(IOneRepository oneRepository)
        {
            _oneRepository = oneRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOneCommand request, CancellationToken cancellationToken)
        {
            var one = await _oneRepository.FindByIdAsync(request.Id, cancellationToken);
            if (one is null)
            {
                throw new NotFoundException($"Could not find One '{request.Id}'");
            }

            one.OneName = request.OneName;
            one.Two.TwoName = request.Two.TwoName;
        }
    }
}