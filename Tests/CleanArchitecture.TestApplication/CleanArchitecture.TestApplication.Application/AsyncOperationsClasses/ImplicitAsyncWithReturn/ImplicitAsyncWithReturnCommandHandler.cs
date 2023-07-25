using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Async;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AsyncOperationsClasses.ImplicitAsyncWithReturn
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
            var entity = await _asyncOperationsClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find AsyncOperationsClass '{request.Id}'");
            }

            await entity.ImplicitWithReturnAsync(cancellationToken);
            return Unit.Value;
        }
    }
}