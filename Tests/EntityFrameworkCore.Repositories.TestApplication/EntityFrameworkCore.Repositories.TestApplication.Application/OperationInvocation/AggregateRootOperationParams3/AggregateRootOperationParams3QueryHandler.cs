using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.OperationInvocation.AggregateRootOperationParams3
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRootOperationParams3QueryHandler : IRequestHandler<AggregateRootOperationParams3Query, SpResultDto>
    {
        private readonly IAggregateRoot1Repository _aggregateRoot1Repository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public AggregateRootOperationParams3QueryHandler(IAggregateRoot1Repository aggregateRoot1Repository,
            IMapper mapper)
        {
            _aggregateRoot1Repository = aggregateRoot1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SpResultDto> Handle(
            AggregateRootOperationParams3Query request,
            CancellationToken cancellationToken)
        {
            var result = _aggregateRoot1Repository.Operation_Params3_ReturnsD_Collection0(new SpParameter(
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
            return result.MapToSpResultDto(_mapper);
        }
    }
}