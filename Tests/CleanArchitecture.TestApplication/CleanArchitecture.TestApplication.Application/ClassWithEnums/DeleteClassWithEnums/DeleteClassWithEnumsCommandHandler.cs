using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ClassWithEnums.DeleteClassWithEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClassWithEnumsCommandHandler : IRequestHandler<DeleteClassWithEnumsCommand>
    {
        private readonly IClassWithEnumsRepository _classWithEnumsRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteClassWithEnumsCommandHandler(IClassWithEnumsRepository classWithEnumsRepository)
        {
            _classWithEnumsRepository = classWithEnumsRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteClassWithEnumsCommand request, CancellationToken cancellationToken)
        {
            var existingClassWithEnums = await _classWithEnumsRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingClassWithEnums is null)
            {
                throw new NotFoundException($"Could not find ClassWithEnums {request.Id}");
            }
            _classWithEnumsRepository.Remove(existingClassWithEnums);
            return Unit.Value;
        }
    }
}