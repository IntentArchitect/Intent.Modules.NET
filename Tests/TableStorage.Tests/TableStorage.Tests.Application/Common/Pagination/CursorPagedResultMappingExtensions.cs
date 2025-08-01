using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.CursorPagedResultMappingExtensions", Version = "1.0")]

namespace TableStorage.Tests.Application.Common.Pagination
{
    public static class CursorPagedResultMappingExtensions
    {
        /// <summary>
        /// For mapping a cursor based paged-list of Domain elements into a page of DTO elements. See <see cref="ICursorPagedList{T}"/>.
        /// </summary>
        /// <param name="pagedList">A single page retrieved from a persistence store.</param>
        /// <param name="mapFunc">
        /// Provide a mapping function where a single Domain element is supplied to the function
        /// that returns a single DTO element. There are some convenient mapping extension methods
        /// available, or alternatively you can instantiate a new DTO element.
        /// <example>results.MapToCursorPagedResult(x => x.MapToItemDTO(_mapper));</example>
        /// <example>results.MapToCursorPagedResult(x => ItemDTO.Create(x.ItemName));</example>
        /// </param>
        /// <typeparam name="TDomain">Domain element type.</typeparam>
        /// <typeparam name="TDto">DTO element type.</typeparam>
        /// <returns>A single page of DTO elements.</returns>
        public static CursorPagedResult<TDto> MapToCursorPagedResult<TDomain, TDto>(
            this ICursorPagedList<TDomain> pagedList,
            Func<TDomain, TDto> mapFunc)
        {
            var data = pagedList.Select(mapFunc).ToList();
            return CursorPagedResult<TDto>.Create(
                pageSize: pagedList.PageSize,
                cursorToken: pagedList.CursorToken,
                data: data);
        }

        /// <summary>
        /// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref="ICursorPagedList{T}"/>.
        /// </summary>
        /// <param name="pagedList">A single page retrieved from a persistence store.</param>
        /// <typeparam name="TDto">DTO element type.</typeparam>
        /// <returns>A single page of DTO elements.</returns>
        public static CursorPagedResult<TDto> MapToCursorPagedResult<TDto>(this ICursorPagedList<TDto> pagedList)
        {
            return CursorPagedResult<TDto>.Create(
                pageSize: pagedList.PageSize,
                cursorToken: pagedList.CursorToken,
                data: pagedList);
        }

        public static Expression<Func<T, bool>> Combine<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(Expression.Invoke(first, param), Expression.Invoke(second, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}