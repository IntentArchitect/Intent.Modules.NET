using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClassWithEnumsCommandHandler : IRequestHandler<UpdateClassWithEnumsCommand>
    {
        private readonly IClassWithEnumsRepository _classWithEnumsRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateClassWithEnumsCommandHandler(IClassWithEnumsRepository classWithEnumsRepository)
        {
            _classWithEnumsRepository = classWithEnumsRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateClassWithEnumsCommand request, CancellationToken cancellationToken)
        {
            var existingClassWithEnums = await _classWithEnumsRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingClassWithEnums is null)
            {
                throw new NotFoundException($"Could not find ClassWithEnums '{request.Id}'");
            }

            existingClassWithEnums.EnumWithDefaultLiteral = request.EnumWithDefaultLiteral;
            existingClassWithEnums.EnumWithoutDefaultLiteral = request.EnumWithoutDefaultLiteral;
            existingClassWithEnums.EnumWithoutValues = request.EnumWithoutValues;
            existingClassWithEnums.NullibleEnumWithDefaultLiteral = request.NullibleEnumWithDefaultLiteral;
            existingClassWithEnums.NullibleEnumWithoutDefaultLiteral = request.NullibleEnumWithoutDefaultLiteral;
            existingClassWithEnums.NullibleEnumWithoutValues = request.NullibleEnumWithoutValues;

        }
    }
}