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

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AggregateRoot1RepoOperationInvocation.Operation_Params3_ReturnsV_Collection0Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params3_ReturnsV_Collection0AsyncCommandHandler : IRequestHandler<Operation_Params3_ReturnsV_Collection0AsyncCommand>
    {
        private readonly IAggregateRoot1Repository _aggregateRoot1Repository;

        [IntentManaged(Mode.Merge)]
        public Operation_Params3_ReturnsV_Collection0AsyncCommandHandler(IAggregateRoot1Repository aggregateRoot1Repository)
        {
            _aggregateRoot1Repository = aggregateRoot1Repository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            Operation_Params3_ReturnsV_Collection0AsyncCommand request,
            CancellationToken cancellationToken)
        {
            await _aggregateRoot1Repository.Operation_Params3_ReturnsV_Collection0Async(
                new SpParameter(
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
                    attributeString: request.AttributeString),
                new AggregateRoot1
                {
                    Tag = request.Tag
                },
                request.StrParam,
                cancellationToken);
        }
    }
}