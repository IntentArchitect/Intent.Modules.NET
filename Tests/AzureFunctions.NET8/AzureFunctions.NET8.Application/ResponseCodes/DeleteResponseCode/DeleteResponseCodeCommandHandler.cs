using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.DeleteResponseCode
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteResponseCodeCommandHandler : IRequestHandler<DeleteResponseCodeCommand>
    {
        private readonly IResponseCodeRepository _responseCodeRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteResponseCodeCommandHandler(IResponseCodeRepository responseCodeRepository)
        {
            _responseCodeRepository = responseCodeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteResponseCodeCommand request, CancellationToken cancellationToken)
        {
            var existingResponseCode = await _responseCodeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingResponseCode is null)
            {
                throw new NotFoundException($"Could not find ResponseCode '{request.Id}'");
            }

            _responseCodeRepository.Remove(existingResponseCode);
        }
    }
}