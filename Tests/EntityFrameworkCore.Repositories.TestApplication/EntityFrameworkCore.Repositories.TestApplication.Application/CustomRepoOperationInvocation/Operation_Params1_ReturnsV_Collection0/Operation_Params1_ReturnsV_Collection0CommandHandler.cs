using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params1_ReturnsV_Collection0
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params1_ReturnsV_Collection0CommandHandler : IRequestHandler<Operation_Params1_ReturnsV_Collection0Command>
    {
        private readonly ICustomRepository _customRepository;

        [IntentManaged(Mode.Merge)]
        public Operation_Params1_ReturnsV_Collection0CommandHandler(ICustomRepository customRepository)
        {
            _customRepository = customRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            Operation_Params1_ReturnsV_Collection0Command request,
            CancellationToken cancellationToken)
        {
            _customRepository.Operation_Params1_ReturnsV_Collection0(new SpParameter(
                attributeBinary: request.AttributeBinary,
                attributeBool: request.AttributeBool,
                attributeByte: request.AttributeByte,
                attributeDate: request.AttributeDate,
                attributeDateTime: request.AttributeDateTime,
                attributeDateTimeOffset: request.AttributeDateTimeOffset,
                attributeDecimal: request.AttributeDecimal,
                attributeDouble: request.AttributeDouble,
                attributeFloat: request.AttributeFloat,
                attributeGuid: request.AttributeGuid,
                attributeInt: request.AttributeInt,
                attributeLong: request.AttributeLong,
                attributeShort: request.AttributeShort,
                attributeString: request.AttributeString));
        }
    }
}