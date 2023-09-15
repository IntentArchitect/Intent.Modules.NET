using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using RichDomain.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Extensions.RepositoryExtensions", Version = "1.0")]

namespace RichDomain.Application.Tests.Extensions
{
    public static class RepositoryExtensions
    {
        public static void OnAdd<TDomain>(this IRepository<TDomain> repository, Action<TDomain> addAction)
        {
            repository.When(x => x.Add(Arg.Any<TDomain>())).Do(ci => addAction(ci.Arg<TDomain>()));
        }
    }
}