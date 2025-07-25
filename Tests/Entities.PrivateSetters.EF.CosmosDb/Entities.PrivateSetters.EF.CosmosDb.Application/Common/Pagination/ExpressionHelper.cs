using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.ExpressionHelper", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application.Common.Pagination
{
    public static class ExpressionHelper
    {
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