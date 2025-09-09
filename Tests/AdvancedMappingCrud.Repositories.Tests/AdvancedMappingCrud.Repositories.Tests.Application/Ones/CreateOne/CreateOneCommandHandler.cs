using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.NullableNested;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.CreateOne
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOneCommandHandler : IRequestHandler<CreateOneCommand, Guid>
    {
        private readonly IOneRepository _oneRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOneCommandHandler(IOneRepository oneRepository)
        {
            _oneRepository = oneRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOneCommand request, CancellationToken cancellationToken)
        {
            var one = new One
            {
                OneName = request.OneName,
                OneAge = 0,
                Two = new Two
                {
                    TwoName = request.Two.TwoName
                }
            };

            _oneRepository.Add(one);
            await _oneRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return one.Id;
        }
    }
}