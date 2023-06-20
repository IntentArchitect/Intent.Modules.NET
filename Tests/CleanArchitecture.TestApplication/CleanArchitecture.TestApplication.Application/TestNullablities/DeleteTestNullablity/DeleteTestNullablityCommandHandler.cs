using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.DeleteTestNullablity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteTestNullablityCommandHandler : IRequestHandler<DeleteTestNullablityCommand>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteTestNullablityCommandHandler(ITestNullablityRepository testNullablityRepository)
        {
            _testNullablityRepository = testNullablityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteTestNullablityCommand request, CancellationToken cancellationToken)
        {
            var existingTestNullablity = await _testNullablityRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingTestNullablity is null)
            {
                throw new NotFoundException($"Could not find TestNullablity {request.Id}");
            }
            _testNullablityRepository.Remove(existingTestNullablity);
            return Unit.Value;
        }
    }
}