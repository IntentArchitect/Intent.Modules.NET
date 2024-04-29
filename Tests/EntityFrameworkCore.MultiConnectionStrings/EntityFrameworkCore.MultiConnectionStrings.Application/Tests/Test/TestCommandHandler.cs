using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Entities;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Application.Tests.Test
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        private readonly IClassARepository _classARepository;
        private readonly IClassBRepository _classBRepository;

        [IntentManaged(Mode.Merge)]
        public TestCommandHandler(IClassARepository classARepository, IClassBRepository classBRepository)
        {
            _classARepository = classARepository;
            _classBRepository = classBRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestCommand request, CancellationToken cancellationToken)
        {
            _classARepository.Add(new ClassA() { Message = request.Message });
            _classBRepository.Add(new ClassB() { Message = request.Message });

            await _classARepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await _classBRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            //throw new Exception("Error");
        }
    }
}