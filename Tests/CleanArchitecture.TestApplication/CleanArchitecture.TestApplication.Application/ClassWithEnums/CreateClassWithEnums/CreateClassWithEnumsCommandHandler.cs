using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.Enums;
using CleanArchitecture.TestApplication.Domain.Repositories.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ClassWithEnums.CreateClassWithEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClassWithEnumsCommandHandler : IRequestHandler<CreateClassWithEnumsCommand, Guid>
    {
        private readonly IClassWithEnumsRepository _classWithEnumsRepository;

        [IntentManaged(Mode.Merge)]
        public CreateClassWithEnumsCommandHandler(IClassWithEnumsRepository classWithEnumsRepository)
        {
            _classWithEnumsRepository = classWithEnumsRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateClassWithEnumsCommand request, CancellationToken cancellationToken)
        {
            var newClassWithEnums = new Domain.Entities.Enums.ClassWithEnums
            {
                EnumWithDefaultLiteral = request.EnumWithDefaultLiteral,
                EnumWithoutDefaultLiteral = request.EnumWithoutDefaultLiteral,
                EnumWithoutValues = request.EnumWithoutValues,
                NullibleEnumWithDefaultLiteral = request.NullibleEnumWithDefaultLiteral,
                NullibleEnumWithoutDefaultLiteral = request.NullibleEnumWithoutDefaultLiteral,
                NullibleEnumWithoutValues = request.NullibleEnumWithoutValues,
#warning No matching field found for CollectionEnum
#warning No matching field found for CollectionStrings
            };

            _classWithEnumsRepository.Add(newClassWithEnums);
            await _classWithEnumsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newClassWithEnums.Id;
        }
    }
}