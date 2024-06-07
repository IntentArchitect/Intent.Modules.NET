using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params1_ReturnsD_Collection1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Operation_Params1_ReturnsD_Collection1QueryHandler : IRequestHandler<Operation_Params1_ReturnsD_Collection1Query, List<SpResultDto>>
    {
        private readonly ICustomRepository _customRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public Operation_Params1_ReturnsD_Collection1QueryHandler(ICustomRepository customRepository, IMapper mapper)
        {
            _customRepository = customRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SpResultDto>> Handle(
            Operation_Params1_ReturnsD_Collection1Query request,
            CancellationToken cancellationToken)
        {
            var result = _customRepository.Operation_Params1_ReturnsD_Collection1(new SpParameter(
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
            return result.MapToSpResultDtoList(_mapper);
        }
    }
}