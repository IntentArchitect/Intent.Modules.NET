using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.OperationInvocation.AggregateRootOperationParams3
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRootOperationParams3CommandHandler : IRequestHandler<AggregateRootOperationParams3Command>
    {
        private readonly IAggregateRoot1Repository _aggregateRoot1Repository;
        [IntentManaged(Mode.Merge)]
        public AggregateRootOperationParams3CommandHandler(IAggregateRoot1Repository aggregateRoot1Repository)
        {
            _aggregateRoot1Repository = aggregateRoot1Repository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AggregateRootOperationParams3Command request, CancellationToken cancellationToken)
        {
            _aggregateRoot1Repository.Operation_Params3_ReturnsV_Collection0(new SpParameter(
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
                attributeString: request.AttributeString), new AggregateRoot1
                {
                    Tag = request.Tag
                }, request.StrParam);
        }
    }
}