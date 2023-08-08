using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Constants.TestApplication.Domain.Common.Exceptions;
using Entities.Constants.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.UpdateTestClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateTestClassCommandHandler : IRequestHandler<UpdateTestClassCommand>
    {
        private readonly ITestClassRepository _testClassRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateTestClassCommandHandler(ITestClassRepository testClassRepository)
        {
            _testClassRepository = testClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateTestClassCommand request, CancellationToken cancellationToken)
        {
            var existingTestClass = await _testClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingTestClass is null)
            {
                throw new NotFoundException($"Could not find TestClass '{request.Id}'");
            }

            existingTestClass.Att100 = request.Att100;
            existingTestClass.VarChar200 = request.VarChar200;
            existingTestClass.NVarChar300 = request.NVarChar300;
            existingTestClass.AttMax = request.AttMax;
            existingTestClass.VarCharMax = request.VarCharMax;
            existingTestClass.NVarCharMax = request.NVarCharMax;

        }
    }
}