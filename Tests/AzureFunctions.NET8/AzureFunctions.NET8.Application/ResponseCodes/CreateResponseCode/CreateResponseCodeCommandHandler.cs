using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Domain.Entities;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.CreateResponseCode
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateResponseCodeCommandHandler : IRequestHandler<CreateResponseCodeCommand, Guid>
    {
        private readonly IResponseCodeRepository _responseCodeRepository;

        [IntentManaged(Mode.Merge)]
        public CreateResponseCodeCommandHandler(IResponseCodeRepository responseCodeRepository)
        {
            _responseCodeRepository = responseCodeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateResponseCodeCommand request, CancellationToken cancellationToken)
        {
            var newResponseCode = new ResponseCode
            {
                Name = request.Name,
            };

            _responseCodeRepository.Add(newResponseCode);
            await _responseCodeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newResponseCode.Id;
        }
    }
}