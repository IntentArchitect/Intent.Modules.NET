using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using CleanArchitecture.TestApplication.Domain.Repositories.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.CreateTestNullablity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTestNullablityCommandHandler : IRequestHandler<CreateTestNullablityCommand, Guid>
    {
        private readonly ITestNullablityRepository _testNullablityRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTestNullablityCommandHandler(ITestNullablityRepository testNullablityRepository)
        {
            _testNullablityRepository = testNullablityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateTestNullablityCommand request, CancellationToken cancellationToken)
        {
            var entity = new TestNullablity(request.Id, request.MyEnum, request.Str, request.Date, request.DateTime, request.NullableGuid, request.NullableEnum, request.DefaultLiteralEnum);

            _testNullablityRepository.Add(entity);
            await _testNullablityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}