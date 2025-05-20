using System.Linq.Expressions;
using EntityFramework.Application.LinqExtensions.Tests.Application;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.Application.LinqExtensions.QueryableMarkerImplementation", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Infrastructure.Repositories
{
    public static class QueryableMarkerImplementation
    {
        public static IQueryable<T> ApplyMarkers<T>(this IQueryable<T> queryable)
            where T : class
        {
            var visitor = new QueryableMarkerExtensions.QueryBehaviorVisitor();
            var visitedExpr = visitor.Visit(queryable.Expression);
            var visited = (IQueryable<T>)visitedExpr.ToQueryable(queryable.Provider);

            if (visitor.NoTracking)
            {
                visited = EntityFrameworkQueryableExtensions.AsNoTracking(visited);
            }

            if (visitor.AsTracking)
            {
                visited = EntityFrameworkQueryableExtensions.AsTracking(visited);
            }

            return visited;
        }

        public static IQueryable ToQueryable(this Expression expression, IQueryProvider provider)
        {
            return provider.CreateQuery(expression);
        }
    }
}