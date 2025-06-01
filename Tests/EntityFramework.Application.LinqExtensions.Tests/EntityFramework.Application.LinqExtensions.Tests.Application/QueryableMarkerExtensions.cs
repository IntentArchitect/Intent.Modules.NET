using System.Linq.Expressions;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.Application.LinqExtensions.QueryableMarkerExtensions", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Application
{
    public static class QueryableMarkerExtensions
    {
        private static readonly MethodInfo AsNoTrackingMethodInfo = typeof(QueryableMarkerExtensions).GetMethod(nameof(AsNoTrackingImpl), BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly MethodInfo AsTrackingMethodInfo = typeof(QueryableMarkerExtensions).GetMethod(nameof(AsTrackingImpl), BindingFlags.NonPublic | BindingFlags.Static)!;

        public static IQueryable<T> AsNoTracking<T>(this IQueryable<T> source)
        {
            return source.Provider.CreateQuery<T>(
                Expression.Call(
                    method: AsNoTrackingMethodInfo.MakeGenericMethod(typeof(T)),
                    arguments: [source.Expression]
                ));
        }

        public static IQueryable<T> AsTracking<T>(this IQueryable<T> source)
        {
            return source.Provider.CreateQuery<T>(
                Expression.Call(
                    method: AsTrackingMethodInfo.MakeGenericMethod(typeof(T)),
                    arguments: [source.Expression]
                ));
        }

        private static IQueryable<T> AsNoTrackingImpl<T>(this IQueryable<T> source) => source;

        private static IQueryable<T> AsTrackingImpl<T>(this IQueryable<T> source) => source;

        public class QueryBehaviorVisitor : ExpressionVisitor
        {
            public bool NoTracking { get; private set; } = false;
            public bool AsTracking { get; private set; } = false;

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(QueryableMarkerExtensions))
                {
                    switch (node.Method.Name)
                    {
                        case nameof(QueryableMarkerExtensions.AsNoTrackingImpl):
                            NoTracking = true;
                            return Visit(node.Arguments[0]);
                        case nameof(QueryableMarkerExtensions.AsTrackingImpl):
                            AsTracking = true;
                            return Visit(node.Arguments[0]);
                    }
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}