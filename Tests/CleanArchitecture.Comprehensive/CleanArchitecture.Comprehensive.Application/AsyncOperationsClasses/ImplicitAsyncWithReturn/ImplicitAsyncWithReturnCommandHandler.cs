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

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ImplicitAsyncWithReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitAsyncWithReturnCommandHandler : IRequestHandler<ImplicitAsyncWithReturnCommand, object>
    {
        private readonly IAsyncOperationsClassRepository _asyncOperationsClassRepository;

        [IntentManaged(Mode.Merge)]
        public ImplicitAsyncWithReturnCommandHandler(IAsyncOperationsClassRepository asyncOperationsClassRepository)
        {
            _asyncOperationsClassRepository = asyncOperationsClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<object> Handle(ImplicitAsyncWithReturnCommand request, CancellationToken cancellationToken)
        {
            var existingAsyncOperationsClass = await _asyncOperationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAsyncOperationsClass is null)
            {
                throw new NotFoundException($"Could not find AsyncOperationsClass '{request.Id}'");
            }

            var result = await existingAsyncOperationsClass.ImplicitWithReturnAsync(cancellationToken);
            return result;

        }
    }
}