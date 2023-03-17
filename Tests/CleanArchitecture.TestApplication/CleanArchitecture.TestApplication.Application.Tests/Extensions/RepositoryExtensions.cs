using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using AutoFixture;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Extensions.RepositoryExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.Extensions
{
    public static class RepositoryExtensions
    {
        public static void OnAdd<TDomain, TPersistence>(this IRepository<TDomain, TPersistence> repository, Action<TDomain> addAction)
        {
            repository.When(x => x.Add(Arg.Any<TDomain>())).Do(ci => addAction(ci.Arg<TDomain>()));
        }

        public static void OnSaveChanges<TDomain, TPersistence>(this IRepository<TDomain, TPersistence> repository, Action saveAction)
        {
            repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => saveAction());
        }

        public static void AutoAssignId<TObj, TId>(this TObj obj, Expression<Func<TObj, TId>> idSelector)
        {
            var fixture = new Fixture();
            var id = fixture.Create<TId>();
            var memberExpr = idSelector.Body as MemberExpression;
            if (memberExpr == null || memberExpr.Member is not PropertyInfo property)
            {
                throw new ArgumentException("Expression must consist of a property only", nameof(idSelector));
            }
            if (property.CanWrite)
            {
                property.SetValue(obj, id, null);
            }
        }
    }
}