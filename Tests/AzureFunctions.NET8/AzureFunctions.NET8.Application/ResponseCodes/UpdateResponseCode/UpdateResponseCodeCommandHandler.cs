using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.UpdateResponseCode
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateResponseCodeCommandHandler : IRequestHandler<UpdateResponseCodeCommand>
    {
        private readonly IResponseCodeRepository _responseCodeRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateResponseCodeCommandHandler(IResponseCodeRepository responseCodeRepository)
        {
            _responseCodeRepository = responseCodeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateResponseCodeCommand request, CancellationToken cancellationToken)
        {
            var existingResponseCode = await _responseCodeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingResponseCode is null)
            {
                throw new NotFoundException($"Could not find ResponseCode '{request.Id}'");
            }

            existingResponseCode.Name = request.Name;
        }
    }
}