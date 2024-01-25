using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Interfaces;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Pagination;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class FindPeopleSCH
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindPeopleSCH(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<PersonDto>> Handle(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var people = await _dbContext.People
                .ToPagedListAsync(pageNo, pageSize, cancellationToken);
            return people.MapToPagedResult(x => x.MapToPersonDto(_mapper));
        }
    }
}