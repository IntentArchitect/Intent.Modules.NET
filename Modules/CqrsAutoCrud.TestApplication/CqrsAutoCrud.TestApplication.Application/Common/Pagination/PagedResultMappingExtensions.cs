using System;
using System.Linq;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.PagedResultMappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.Common.Pagination
{
    public static class PagedResultMappingExtensions
    {
        /// <summary>
        /// Maps a page of Domain elements into a page of DTO elements. See <see cref="IPagedResult{T}"/>. 
        /// </summary>
        /// <param name="pagedResult">A single page retrieved from a persistence store.</param>
        /// <param name="mapFunc"> 
        /// Provide a mapping function where a single Domain element is supplied to the function
        /// that returns a single DTO element. There are some convenient mapping extension methods
        /// available or alternatively you can instantiate a new DTO element.
        /// <example>results.MapToPagedResult(x => x.MapToItemDTO(_mapper));</example>
        /// <example>results.MapToPagedResult(x => ItemDTO.Create(x.ItemName));</example>
        /// </param>
        /// <typeparam name="TDomain">Domain element type</typeparam>
        /// <typeparam name="TDto">DTO element type</typeparam>
        /// <returns>A single page of DTO elements</returns>
        public static PagedResult<TDto> MapToPagedResult<TDomain, TDto>(this IPagedResult<TDomain> pagedResult, Func<TDomain, TDto> mapFunc)
        {
            var data = pagedResult.Select(mapFunc).ToList();

            return PagedResult<TDto>.Create(
                totalCount: pagedResult.TotalCount,
                pageCount: pagedResult.PageCount,
                pageSize: pagedResult.PageSize,
                pageNumber: pagedResult.PageNo,
                data: data);
        }
    }

}