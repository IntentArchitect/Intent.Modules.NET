using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Constants.TestApplication.Domain.Entities;
using Entities.Constants.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.CreateTestClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTestClassCommandHandler : IRequestHandler<CreateTestClassCommand, Guid>
    {
        private readonly ITestClassRepository _testClassRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTestClassCommandHandler(ITestClassRepository testClassRepository)
        {
            _testClassRepository = testClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateTestClassCommand request, CancellationToken cancellationToken)
        {
            var newTestClass = new TestClass
            {
                Att100 = request.Att100,
                VarChar200 = request.VarChar200,
                NVarChar300 = request.NVarChar300,
                AttMax = request.AttMax,
                VarCharMax = request.VarCharMax,
                NVarCharMax = request.NVarCharMax,
            };

            _testClassRepository.Add(newTestClass);
            await _testClassRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newTestClass.Id;
        }
    }
}