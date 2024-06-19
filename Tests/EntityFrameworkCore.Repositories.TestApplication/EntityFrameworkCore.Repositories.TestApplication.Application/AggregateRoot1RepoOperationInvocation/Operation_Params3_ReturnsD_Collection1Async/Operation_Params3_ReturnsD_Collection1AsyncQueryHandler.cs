using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AggregateRoot1RepoOperationInvocation.Operation_Params3_ReturnsD_Collection1Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params3_ReturnsD_Collection1AsyncQueryHandler : IRequestHandler<Operation_Params3_ReturnsD_Collection1AsyncQuery, List<SpResultDto>>
    {
        private readonly IAggregateRoot1Repository _aggregateRoot1Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public Operation_Params3_ReturnsD_Collection1AsyncQueryHandler(IAggregateRoot1Repository aggregateRoot1Repository,
            IMapper mapper)
        {
            _aggregateRoot1Repository = aggregateRoot1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SpResultDto>> Handle(
            Operation_Params3_ReturnsD_Collection1AsyncQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _aggregateRoot1Repository.Operation_Params3_ReturnsD_Collection1Async(
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
            return result.MapToSpResultDtoList(_mapper);
        }
    }
}