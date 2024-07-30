using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.PagingTS;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PagingTSService : IPagingTSService
    {
        private readonly IPagingTSRepository _pagingTSRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public PagingTSService(IPagingTSRepository pagingTSRepository, IMapper mapper)
        {
            _pagingTSRepository = pagingTSRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreatePagingTS(PagingTSCreateDto dto, CancellationToken cancellationToken = default)
        {
            var pagingTS = new Domain.Entities.PagingTS
            {
                Name = dto.Name
            };

            _pagingTSRepository.Add(pagingTS);
            await _pagingTSRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return pagingTS.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagingTSDto> FindPagingTSById(Guid id, CancellationToken cancellationToken = default)
        {
            var pagingTS = await _pagingTSRepository.FindByIdAsync(id, cancellationToken);
            if (pagingTS is null)
            {
                throw new NotFoundException($"Could not find PagingTS '{id}'");
            }
            return pagingTS.MapToPagingTSDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<PagingTSDto>> FindPagingTS(
            int pageNo,
            int pageSize,
            string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var pagingTS = await _pagingTSRepository.FindAllAsync(pageNo, pageSize, queryOptions => queryOptions.OrderBy(orderBy ?? "Id"), cancellationToken);
            return pagingTS.MapToPagedResult(x => x.MapToPagingTSDto(_mapper));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdatePagingTS(Guid id, PagingTSUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var pagingTS = await _pagingTSRepository.FindByIdAsync(id, cancellationToken);
            if (pagingTS is null)
            {
                throw new NotFoundException($"Could not find PagingTS '{id}'");
            }

            pagingTS.Name = dto.Name;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeletePagingTS(Guid id, CancellationToken cancellationToken = default)
        {
            var pagingTS = await _pagingTSRepository.FindByIdAsync(id, cancellationToken);
            if (pagingTS is null)
            {
                throw new NotFoundException($"Could not find PagingTS '{id}'");
            }

            _pagingTSRepository.Remove(pagingTS);
        }

        public void Dispose()
        {
        }
    }
}