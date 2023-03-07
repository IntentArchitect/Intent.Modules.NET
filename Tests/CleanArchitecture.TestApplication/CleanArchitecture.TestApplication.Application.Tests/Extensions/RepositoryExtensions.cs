using System;
using System.Threading;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;

[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.RepositoryExtensions", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Application.Tests.Extensions;

public static class RepositoryExtensions
{
    public static void OnAdd<TDomain, TPersistence>(this IRepository<TDomain, TPersistence> repository, Action<TDomain> addAction)
    {
        repository.When(x => x.Add(Arg.Any<TDomain>())).Do(ci => addAction(ci.Arg<TDomain>()));
    }

    public static void OnSave<TDomain, TPersistence>(this IRepository<TDomain, TPersistence> repository, Action saveAction)
    {
        repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => saveAction());
    }
}