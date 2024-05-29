using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.GetClassWithEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClassWithEnumsQueryHandler : IRequestHandler<GetClassWithEnumsQuery, List<ClassWithEnumsDto>>
    {
        private readonly IClassWithEnumsRepository _classWithEnumsRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClassWithEnumsQueryHandler(IClassWithEnumsRepository classWithEnumsRepository, IMapper mapper)
        {
            _classWithEnumsRepository = classWithEnumsRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClassWithEnumsDto>> Handle(
            GetClassWithEnumsQuery request,
            CancellationToken cancellationToken)
        {
            var classWithEnums = await _classWithEnumsRepository.FindAllAsync(cancellationToken);
            return classWithEnums.MapToClassWithEnumsDtoList(_mapper);
        }
    }
}