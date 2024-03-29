using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Constants.TestApplication.Domain.Common.Exceptions;
using Entities.Constants.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.DeleteTestClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteTestClassCommandHandler : IRequestHandler<DeleteTestClassCommand>
    {
        private readonly ITestClassRepository _testClassRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteTestClassCommandHandler(ITestClassRepository testClassRepository)
        {
            _testClassRepository = testClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteTestClassCommand request, CancellationToken cancellationToken)
        {
            var existingTestClass = await _testClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingTestClass is null)
            {
                throw new NotFoundException($"Could not find TestClass '{request.Id}'");
            }

            _testClassRepository.Remove(existingTestClass);

        }
    }
}