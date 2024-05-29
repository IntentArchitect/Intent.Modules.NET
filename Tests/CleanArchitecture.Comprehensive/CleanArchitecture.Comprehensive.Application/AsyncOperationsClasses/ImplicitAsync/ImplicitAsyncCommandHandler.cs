using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Async;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ImplicitAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitAsyncCommandHandler : IRequestHandler<ImplicitAsyncCommand>
    {
        private readonly IAsyncOperationsClassRepository _asyncOperationsClassRepository;

        [IntentManaged(Mode.Merge)]
        public ImplicitAsyncCommandHandler(IAsyncOperationsClassRepository asyncOperationsClassRepository)
        {
            _asyncOperationsClassRepository = asyncOperationsClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ImplicitAsyncCommand request, CancellationToken cancellationToken)
        {
            var existingAsyncOperationsClass = await _asyncOperationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAsyncOperationsClass is null)
            {
                throw new NotFoundException($"Could not find AsyncOperationsClass '{request.Id}'");
            }

            await existingAsyncOperationsClass.ImplicitAsync(cancellationToken);

        }
    }
}