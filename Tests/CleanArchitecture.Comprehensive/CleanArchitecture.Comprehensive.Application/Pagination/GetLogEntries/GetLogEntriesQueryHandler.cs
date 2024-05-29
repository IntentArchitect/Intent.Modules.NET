using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination.GetLogEntries
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetLogEntriesQueryHandler : IRequestHandler<GetLogEntriesQuery, PagedResult<LogEntryDto>>
    {
        private readonly ILogEntryRepository _logEntryRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetLogEntriesQueryHandler(ILogEntryRepository logEntryRepository, IMapper mapper)
        {
            _logEntryRepository = logEntryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<LogEntryDto>> Handle(GetLogEntriesQuery request, CancellationToken cancellationToken)
        {
            var results = await _logEntryRepository.FindAllAsync(
                pageNo: request.PageNo,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapToLogEntryDto(_mapper));
        }
    }
}